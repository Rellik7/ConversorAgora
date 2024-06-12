using Autofac.Extras.Moq;
using ConversorAgora.BO;
using ConversorAgora.Utils;
using Moq;
using Moq.Protected;
using System.Text;

namespace ConversorAgora.Test.BO
{
    public class ConversorConstrutor
    {
        [Theory]
        [InlineData("MINHA CDN", @"https://input-01.txt", @"C:\", true)]
        [InlineData("", @"https://input-01.txt", @"C:\", false)]
        [InlineData(null, @"https://input-01.txt", @"C:\", false)]
        [InlineData("MINHA CDN", "", @"C:\", false)]
        [InlineData("MINHA CDN", null, @"C:\", false)]
        [InlineData("MINHA CDN", @"https://input-01.txt", "", false)]
        [InlineData("MINHA CDN", @"https://input-01.txt", null, false)]
        public void RetornaEhValidoDeAcordoComDadosDeEntrada(string provider, string sourceURL, string targetPath, bool validacao)
        {
            using AutoMock mock = AutoMock.GetLoose();
            string mockFileContent = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2";
            byte[] mockBytes = Encoding.UTF8.GetBytes(mockFileContent);
            MemoryStream mockMemoryStream = new MemoryStream(mockBytes);
            mock.Mock<IFileManager>()
                .Setup(fileManager => fileManager.StreamReader(It.IsAny<Stream>()))
                .Returns(() => new StreamReader(mockMemoryStream));

            Conversor conversor = new(provider, sourceURL, targetPath, mock.Create<IFileManager>(), GetMockClient(sourceURL));

            Assert.Equal(validacao, conversor.EhValido);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RetornaMensagemDeErroDeProviderInvalidoQuandoProviderNuloOuVazio(string provider)
        {
            using AutoMock mock = AutoMock.GetLoose();
            string mockFileContent = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2";
            byte[] mockBytes = Encoding.UTF8.GetBytes(mockFileContent);
            MemoryStream mockMemoryStream = new MemoryStream(mockBytes);
            mock.Mock<IFileManager>()
                .Setup(fileManager => fileManager.StreamReader(It.IsAny<Stream>()))
                .Returns(() => new StreamReader(mockMemoryStream));

            string sourceURL = @"C:\";
            string targetPath = @"C\Git\output-01.txt";

            Conversor conversor = new(provider, sourceURL, targetPath, mock.Create<IFileManager>(), GetMockClient(sourceURL));

            Assert.Contains("Provedor inválido.", conversor.Erros.Sumario);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(@"Z:\Test\Testes")]
        public void RetornaMensagemDeErroDeSourcePathInvalidoQuandoSourcePathNuloOuVazioOuInexistente(string sourceURL)
        {
            string provider = "MINHA CDN";
            string targetPath = @"C:\";

            Conversor conversor = new(provider, sourceURL, targetPath, null, GetMockClient(sourceURL));

            Assert.Contains("Caminho do arquivo fonte inválido.", conversor.Erros.Sumario);
        }

        [Fact]
        public void RetornaMensagemDeErroDeArquivoVazioQuandoArquivoVazio()
        {
            using AutoMock mock = AutoMock.GetLoose();
            string mockFileContent = "";
            byte[] mockBytes = Encoding.UTF8.GetBytes(mockFileContent);
            MemoryStream mockMemoryStream = new MemoryStream(mockBytes);
            mock.Mock<IFileManager>()
                .Setup(fileManager => fileManager.StreamReader(It.IsAny<Stream>()))
                .Returns(() => new StreamReader(mockMemoryStream));

            string provider = "MINHA CDN";
            string sourceURL = "";
            string targetPath = @"C:\";

            Conversor conversor = new(provider, sourceURL, targetPath, null, GetMockClient(sourceURL));

            Assert.Contains("Caminho do arquivo fonte inválido.", conversor.Erros.Sumario);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RetornaMensagemDeErroDeTargetPathInvalidoQuandoTargetPathNuloOuVazio(string targetPath)
        {
            string provider = "MINHA CDN";
            string sourceURL = @"https://input-01.txt";

            Conversor conversor = new(provider, sourceURL, targetPath, null, GetMockClient(sourceURL));

            Assert.Contains("Caminho do arquivo alvo inválido.", conversor.Erros.Sumario);
        }

        public static HttpClient GetMockClient(string sourceURL)
        {
            Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            HttpResponseMessage httpResponseMessage = new()
            {
                Content = new StringContent("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2")
            };

            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            return new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new System.Uri(sourceURL)
            };
        }
    }
}
