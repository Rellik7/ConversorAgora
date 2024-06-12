using Autofac.Extras.Moq;
using ConversorAgora.BO;
using ConversorAgora.Utils;
using Moq;
using System.Text;

namespace ConversorAgora.Test.BO
{
    public class ConversorConstrutor
    {
        [Theory]
        [InlineData("MINHA CDN", @"C:\", @"C:\", true)]
        [InlineData("", @"C:\", @"C:\", false)]
        [InlineData(null, @"C:\", @"C:\", false)]
        [InlineData("MINHA CDN", "", @"C:\", false)]
        [InlineData("MINHA CDN", null, @"C:\", false)]
        [InlineData("MINHA CDN", @"C:\", "", false)]
        [InlineData("MINHA CDN", @"C:\", null, false)]
        public void RetornaEhValidoDeAcordoComDadosDeEntrada(string provider, string sourceURL, string targetPath, bool validacao)
        {
            using AutoMock mock = AutoMock.GetLoose();
            string mockFileContent = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2";
            byte[] mockBytes = Encoding.UTF8.GetBytes(mockFileContent);
            MemoryStream mockMemoryStream = new MemoryStream(mockBytes);
            mock.Mock<IFileManager>()
                .Setup(fileManager => fileManager.StreamReader(It.IsAny<string>()))
                .Returns(() => new StreamReader(mockMemoryStream));

            Conversor conversor = new(provider, sourceURL, targetPath, mock.Create<IFileManager>());

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
                .Setup(fileManager => fileManager.StreamReader(It.IsAny<string>()))
                .Returns(() => new StreamReader(mockMemoryStream));

            string sourceURL = @"C:\";
            string targetPath = @"C\Git\output-01.txt";

            Conversor conversor = new(provider, sourceURL, targetPath, mock.Create<IFileManager>());

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

            Conversor conversor = new(provider, sourceURL, targetPath, null);

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
                .Setup(fileManager => fileManager.StreamReader(It.IsAny<string>()))
                .Returns(() => new StreamReader(mockMemoryStream));

            string provider = "MINHA CDN";
            string sourceURL = "";
            string targetPath = @"C:\";

            Conversor conversor = new(provider, sourceURL, targetPath, null);

            Assert.Contains("Caminho do arquivo fonte inválido.", conversor.Erros.Sumario);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RetornaMensagemDeErroDeTargetPathInvalidoQuandoTargetPathNuloOuVazio(string targetPath)
        {
            string provider = "MINHA CDN";
            string sourceURL = @"C\";

            Conversor conversor = new(provider, sourceURL, targetPath, null);

            Assert.Contains("Caminho do arquivo alvo inválido.", conversor.Erros.Sumario);
        }
    }
}
