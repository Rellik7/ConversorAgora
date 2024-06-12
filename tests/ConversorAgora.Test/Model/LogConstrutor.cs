using ConversorAgora.Model;

namespace ConversorAgora.Test.Model
{
    public class LogConstrutor
    {
        [Theory]
        [InlineData("MINHA CDN", "GET", "200", "/robots.txt", "100.2", "312", "HIT", true)]
        [InlineData("MINHA CDN", "POST", "100", "/myImages", "319.4", "101", "MISS", true)]
        [InlineData("MINHA CDN", "POST", "599", "/myImages", "319.4", "101", "MISS", true)]
        [InlineData("MINHA CDN", "POST", "800", "/myImages", "319.4", "101", "MISS", false)]
        [InlineData("MINHA CDN", "POST", "50", "/myImages", "319.4", "101", "MISS", false)]
        [InlineData(null, "POST", "200", "/myImages", "319.4", "101", "MISS", false)]
        [InlineData("MINHA CDN", null, "200", "/robots.txt", "100.2", "312", "MISS", false)]
        [InlineData("MINHA CDN", "POST", null, "/myImages", "319.4", "101", "MISS", false)]
        [InlineData("MINHA CDN", "POST", "200", null, "319.4", "101", "MISS", false)]
        [InlineData("MINHA CDN", "POST", "200", "/myImages", null, "101", "MISS", false)]
        [InlineData("MINHA CDN", "POST", "200", "/myImages", "319.4", null, "MISS", false)]
        [InlineData("MINHA CDN", "POST", "200", "/myImages", "319.4", "101", null, false)]
        public void RetornaEhValidoDeAcordoComDadosDeEntrada(string provider, string httpMethod, string statusCode, string uriPath, string timeTaken, string responseSize, string cacheStatus, bool validacao)
        {
            Log log = new(provider, httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus);

            Assert.Equal(validacao, log.EhValido);
        }

        [Fact]
        public void RetornaMensagemDeErroDeProvedorInvalidoQuandoProvedorNuloOuVazio()
        {
            string provider = null;
            string httpMethod = "GET";
            string statusCode = "200";
            string uriPath = "/robots.txt";
            string timeTaken = "100.2";
            string responseSize = "312";
            string cacheStatus = "HIT";
            Log log = new(provider, httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus);

            Assert.Contains("Provedor inválido.", log.Erros.Sumario);
            Assert.False(log.EhValido);
        }

        [Theory]
        [InlineData("50")]
        [InlineData("800")]
        [InlineData("600")]
        [InlineData("")]
        [InlineData(null)]
        public void RetornaMensagemDeErroDeHttpMethodInvalidoQuandoHttpMethodVazioOuNuloOuMaiorOuMenorQueOPermitido(string httpMethod)
        {
            string provider = "MINHA CDN";
            string statusCode = "200";
            string uriPath = "/robots.txt";
            string timeTaken = "100.2";
            string responseSize = "312";
            string cacheStatus = "HIT";
            Log log = new(provider, httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus);

            Assert.Contains("Método HTTP inválido.", log.Erros.Sumario);
            Assert.False(log.EhValido);
        }

        [Fact]
        public void RetornaMensagemDeErroDeStatusCodeInvalidoQuandoStatusCodeNuloOuVazio()
        {
            string provider = "MINHA CDN";
            string httpMethod = "GET";
            string statusCode = null;
            string uriPath = "/robots.txt";
            string timeTaken = "100.2";
            string responseSize = "312";
            string cacheStatus = "HIT";
            Log log = new(provider, httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus);

            Assert.Contains("Código de status inválido.", log.Erros.Sumario);
            Assert.False(log.EhValido);
        }

        [Fact]
        public void RetornaMensagemDeErroDeUriPathInvalidoQuandoUriPathNuloOuVazio()
        {
            string provider = "MINHA CDN";
            string httpMethod = "GET";
            string statusCode = "200";
            string uriPath = null;
            string timeTaken = "100.2";
            string responseSize = "312";
            string cacheStatus = "HIT";
            Log log = new(provider, httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus);

            Assert.Contains("Caminho uri inválido.", log.Erros.Sumario);
            Assert.False(log.EhValido);
        }

        [Fact]
        public void RetornaMensagemDeErroDeTimeTakenInvalidoQuandoTimeTakenNuloOuVazio()
        {
            string provider = "MINHA CDN";
            string httpMethod = "GET";
            string statusCode = "200";
            string uriPath = "/robots.txt";
            string timeTaken = null;
            string responseSize = "312";
            string cacheStatus = "HIT";
            Log log = new(provider, httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus);

            Assert.Contains("Tempo informado inválido.", log.Erros.Sumario);
            Assert.False(log.EhValido);
        }

        [Fact]
        public void RetornaMensagemDeErroDeResponseSizeInvalidoQuandoResponseSizeNuloOuVazio()
        {
            string provider = "MINHA CDN";
            string httpMethod = "GET";
            string statusCode = "200";
            string uriPath = "/robots.txt";
            string timeTaken = "100.2";
            string responseSize = null;
            string cacheStatus = "HIT";
            Log log = new(provider, httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus);

            Assert.Contains("Tamanho da resposta inválido.", log.Erros.Sumario);
            Assert.False(log.EhValido);
        }

        [Fact]
        public void RetornaMensagemDeErroDeCacheStatusInvalidoQuandoCacheStatusNuloOuVazio()
        {
            string provider = "MINHA CDN";
            string httpMethod = "GET";
            string statusCode = "200";
            string uriPath = "/robots.txt";
            string timeTaken = "100.2";
            string responseSize = "312";
            string cacheStatus = null;
            Log log = new(provider, httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus);

            Assert.Contains("Status de cache inválido.", log.Erros.Sumario);
            Assert.False(log.EhValido);
        }
    }
}