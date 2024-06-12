using ConversorAgora.Utils;

namespace ConversorAgora.Model
{
    public class Log: Valida
    {
        private enum MetodosHTTP
        {
            GET,
            HEAD,
            POST,
            PUT,
            DELETE,
            CONNECT,
            OPTIONS,
            TRACE,
            PATCH
        };
        public string Provider { get; set; }
        public string HttpMethod { get; set; }
        public string StatusCode { get; set; }
        public string UriPath { get; set; }
        public string TimeTaken { get; set; }
        public string ResponseSize { get; set; }
        public string CacheStatus { get; set; }

        public Log(string provider, string httpMethod, string statusCode, string uriPath, string timeTaken, string responseSize, string cacheStatus)
        {
            Provider = provider;
            HttpMethod = httpMethod;
            StatusCode = statusCode;
            UriPath = uriPath;
            TimeTaken = timeTaken?[..3];
            ResponseSize = responseSize;
            CacheStatus = cacheStatus;
            Validar();
        }

        protected override void Validar()
        {
            if (string.IsNullOrEmpty(Provider)) Erros.RegistrarErro("Provedor inválido.");
            if (!ValidarHttpMethod(HttpMethod)) Erros.RegistrarErro("Método HTTP inválido.");
            if (!ValidarStatusCode(StatusCode)) Erros.RegistrarErro("Código de status inválido.");
            if (string.IsNullOrEmpty(UriPath)) Erros.RegistrarErro("Caminho uri inválido.");
            if (string.IsNullOrEmpty(TimeTaken)) Erros.RegistrarErro("Tempo informado inválido.");
            if (string.IsNullOrEmpty(ResponseSize)) Erros.RegistrarErro("Tamanho da resposta inválido.");
            if (string.IsNullOrEmpty(CacheStatus)) Erros.RegistrarErro("Status de cache inválido.");
        }

        private static bool ValidarStatusCode(string statusCode)
        {
            return int.TryParse(statusCode, out int code) && code >= 100 && code <= 599;
        }

        private static bool ValidarHttpMethod(string httpMethod)
        {
            return !string.IsNullOrEmpty(httpMethod) && Enum.GetNames(typeof(MetodosHTTP)).Any(e => httpMethod.Contains(e));
        }
    }
}
