using Autofac.Extras.Moq;
using ConversorAgora.BO;
using ConversorAgora.Utils;
using ConversorAgora.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.Protected;

namespace ConversorAgora.Test.BO
{
    public class ConversorConverterLog
    {
        [Fact]
        public void JogaExcecaoQuandoNaoEPossivelCriarCaminhoAlvo()
        {
            string provider = "MinhaCDN";
            string sourceURL = @"https://input-01.txt";
            string targetPath = @"Z:\Teste.txt";

            using AutoMock mock = AutoMock.GetLoose();
            string mockFileContent = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2";
            byte[] mockBytes = Encoding.UTF8.GetBytes(mockFileContent);
            MemoryStream mockMemoryStream = new MemoryStream(mockBytes);
            mock.Mock<IFileManager>()
                .Setup(fileManager => fileManager.StreamReader(It.IsAny<Stream>()))
                .Returns(() => new StreamReader(mockMemoryStream));
            mock.Mock<IFileManager>()
                .Setup(fileManager => fileManager.StreamWriter(targetPath))
                .Returns(() => new StreamWriter(targetPath));

            Conversor conversor = new(provider, sourceURL, targetPath, mock.Create<IFileManager>(), ConversorConstrutor.GetMockClient(sourceURL));

            Assert.Throws<ArgumentException>(() => conversor.ConverterLog());
        }
    }
}
