using ConversorAgora.Model;
using ConversorAgora.Utils;
using System.Text.RegularExpressions;

namespace ConversorAgora.BO
{
    public class Conversor: Valida
    {
        private readonly IFileManager fileManager;
        private readonly HttpClient sourceClient;
        private readonly string provider;
        private readonly string sourceURL;
        private readonly string targetPath;
        private readonly List<Log> logs;
        private string? logAgora;

        public Conversor(string provider, string sourceURL, string targetPath, IFileManager fileManager, HttpClient sourceClient)
        {
            this.fileManager = fileManager;
            this.sourceClient = sourceClient;
            logs = new List<Log>();
            this.provider = provider;
            this.sourceURL = sourceURL;
            this.targetPath = targetPath;
            Validar();
        }

        public void ConverterLog()
        {
            CarregarLogMinhaCDN();
            GerarLogAgora();
            SalvarArquivoDestino();
        }

        private void SalvarArquivoDestino()
        {
            try
            {
                using var outputWriter = fileManager.StreamWriter(targetPath);
                outputWriter.Write(logAgora);
            }
            catch (Exception e)
            {
                if (e.GetType().ToString().Contains("System.IO.DirectoryNotFoundException"))
                {
                    throw new Exception("Não foi possível criar o arquivo no caminho especificado, verifique o parâmetro e tente novamente.");
                }
                else
                {
                    throw new Exception($"Erro não identificado, contate o administrador: \r\n{e.Message}");
                }
            }
        }

        private void GerarLogAgora()
        {
            logAgora = $"#Version: 1.0\r\n#Date: {DateTime.Now}\r\n#Fields: provider http-method status-code uri-path time-taken response-size cache-status";
            foreach (Log log in logs)
            {
                logAgora += $"\r\n{log.Provider} {log.HttpMethod} {log.StatusCode} {log.UriPath} {log.TimeTaken} {log.ResponseSize} {log.CacheStatus}";
            }
        }

        public void CarregarLogMinhaCDN()
        {
            using var sr = fileManager.StreamReader(GetSourceContent());
            while(!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (line == null) return;

                string pattern = "(^\\d{3})\\|(\\d{3})\\|(\\w*)\\|\"(\\w*)\\s(\\/\\S*)\\s.*\"\\|(\\d*)";
                MatchCollection matches = Regex.Matches(line, pattern);

                string httpMethod = matches[0].Groups[4].Value;
                string statusCode = matches[0].Groups[2].Value;
                string uriPath = matches[0].Groups[5].Value;
                string timeTaken = matches[0].Groups[6].Value;
                string responseSize = matches[0].Groups[1].Value;
                string cacheStatus = matches[0].Groups[3].Value;
                logs.Add(new($"\"{provider}\"", httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus));
            }
        }

        private Stream GetSourceContent()
        {
            HttpResponseMessage content = sourceClient.GetAsync(sourceURL).Result;
            return content.Content.ReadAsStream();
        }

        protected override void Validar()
        {
            if (string.IsNullOrEmpty(provider)) Erros.RegistrarErro("Provedor inválido.");

            if (ValidarSourceURL()) ValidarArquivoVazio();

            if (string.IsNullOrEmpty(targetPath)) Erros.RegistrarErro("Caminho do arquivo destino inválido.");
        }

        private bool ValidarSourceURL()
        {
            try
            {
                return GetSourceContent() != null;
            }
            catch (Exception)
            {
                Erros.RegistrarErro("URL do arquivo fonte inválido.");
                return false;
            }
        }

        private void ValidarArquivoVazio()
        {
            using var sr = fileManager.StreamReader(GetSourceContent());
            if (string.IsNullOrEmpty(sr.ReadToEnd())) Erros.RegistrarErro("Arquivo informado está vazio.");
        }

        public void ValidarLogs()
        {
            foreach (Log log in logs)
            {
                if (!log.EhValido) Erros.RegistrarErro(log.Erros.Sumario);
            }
        }
    }
}
