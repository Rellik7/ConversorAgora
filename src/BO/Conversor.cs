using ConversorAgora.Model;
using ConversorAgora.Utils;
using System.Text.RegularExpressions;

namespace ConversorAgora.BO
{
    public class Conversor: Valida
    {
        private readonly string provider;
        private readonly string sourceURL;
        private readonly string targetPath;
        private readonly List<Log> logs;
        private readonly string cabecalho;

        public Conversor(string provider, string sourceURL, string targetPath)
        {
            logs = new List<Log>();
            this.provider = provider;
            this.sourceURL = sourceURL;
            this.targetPath = targetPath;
            cabecalho = $"#Version: 1.0\r\n#Date: {DateTime.Now}\r\n#Fields: provider http-method status-code uri-path time-taken response-size cache-status";
        }

        public void ConverterLog()
        {
            CarregarLogMinhaCDN();
            GerarLogAgora();
        }

        private void GerarLogAgora()
        {
            string output = cabecalho;
            foreach (Log log in logs)
            {
                output += $"\r\n{log.Provider} {log.HttpMethod} {log.StatusCode} {log.UriPath} {log.TimeTaken} {log.ResponseSize} {log.CacheStatus}";
            }
            
            using StreamWriter outputWriter = new(targetPath);
            outputWriter.Write(output);
        }

        public void CarregarLogMinhaCDN()
        {
            using StreamReader sr = new(sourceURL);
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
                Log log = new($"\"{provider}\"", httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus);
                logs.Add(log);
            }
        }

        protected override void Validar()
        {
            if (string.IsNullOrEmpty(provider)) Erros.RegistrarErro("Provedor inválido.");
            if (!Path.Exists(sourceURL)) Erros.RegistrarErro("Caminho do arquivo fonte inválido.");
            if (string.IsNullOrEmpty(targetPath)) Erros.RegistrarErro("Caminho do arquivo alvo inválido.");

            foreach (Log log in logs)
            {
                if (log.EhValido) Erros.RegistrarErro(log.Erros.Sumario);
            }
        }
    }
}
