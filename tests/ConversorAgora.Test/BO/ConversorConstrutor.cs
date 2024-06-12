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
            using AutoMock mock = GetFileManagerMock("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2");

            Conversor conversor = new(provider, sourceURL, targetPath, mock.Create<IFileManager>(), GetClientMock(sourceURL));

            Assert.Equal(validacao, conversor.EhValido);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RetornaMensagemDeErroDeProviderInvalidoQuandoProviderNuloOuVazio(string provider)
        {
            using AutoMock mock = GetFileManagerMock("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2");

            string sourceURL = @"C:\";
            string targetPath = @"C\Git\output-01.txt";

            Conversor conversor = new(provider, sourceURL, targetPath, mock.Create<IFileManager>(), GetClientMock(sourceURL));

            Assert.Contains("Provedor inválido.", conversor.Erros.Sumario);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RetornaMensagemDeErroDeSourcePathInvalidoQuandoSourcePathNuloOuVazio(string sourceURL)
        {
            string provider = "MINHA CDN";
            string targetPath = @"C:\";

            Conversor conversor = new(provider, sourceURL, targetPath, GetFileManagerMock("").Create<IFileManager>(), GetClientMock(sourceURL));

            Assert.Contains("URL do arquivo fonte inválido.", conversor.Erros.Sumario);
        }

        [Fact]
        public void RetornaMensagemDeErroDeArquivoVazioQuandoArquivoVazio()
        {
            string provider = "MINHA CDN";
            string sourceURL = @"https://input-01.txt";
            string targetPath = @"C:\";

            Conversor conversor = new(provider, sourceURL, targetPath, GetFileManagerMock("").Create<IFileManager>(), GetClientMock(sourceURL));

            Assert.Contains("Arquivo informado está vazio.", conversor.Erros.Sumario);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RetornaMensagemDeErroDeTargetPathInvalidoQuandoTargetPathNuloOuVazio(string targetPath)
        {
            string provider = "MINHA CDN";
            string sourceURL = @"https://input-01.txt";

            Conversor conversor = new(provider, sourceURL, targetPath, GetFileManagerMock("").Create<IFileManager>(), GetClientMock(sourceURL));

            Assert.Contains("Caminho do arquivo destino inválido.", conversor.Erros.Sumario);
        }

        public static AutoMock GetFileManagerMock(string mockFileContent)
        {
            AutoMock mock = AutoMock.GetLoose();
            byte[] mockBytes = Encoding.UTF8.GetBytes(mockFileContent);
            MemoryStream mockMemoryStream = new(mockBytes);
            mock.Mock<IFileManager>()
                .Setup(fileManager => fileManager.StreamReader(It.IsAny<Stream>()))
                .Returns(() => new StreamReader(mockMemoryStream));
            return mock;
        }

        public static HttpClient GetClientMock(string sourceURL)
        {
            Mock<HttpMessageHandler> httpMessageHandlerMock = new();

            HttpResponseMessage httpResponseMessage = new()
            {
                Content = new StringContent("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2")
            };

            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            if (!string.IsNullOrEmpty(sourceURL)) 
            {
                return new HttpClient(httpMessageHandlerMock.Object)
                {
                    BaseAddress = new Uri(sourceURL)
                };
            }
            else
            {
                return new HttpClient(httpMessageHandlerMock.Object);
            }
        }
    }
}
