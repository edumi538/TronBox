using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TronBox.API.Tests.Config;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using Xunit;

namespace TronBox.API.Tests
{
    [TestCaseOrderer("TronBox.API.Tests.Config.PriorityOrderer", "TronBox.API.Tests")]
    [Collection(nameof(IntegrationTestsFixtureCollection))]
    public class UploadApiTests
    {
        private readonly IntegrationTestsFixture<StartupTests> _testsFixture;
        internal const string URL_BASE = "api/v1/enviar-documentos";

        public UploadApiTests(IntegrationTestsFixture<StartupTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Upload novo documento válido")]
        [Trait("Upload", "Integração API - Single Válido")]
        public async Task Upload_NovoArquivo_DeveRetornarComSucesso()
        {
            // Arrange
            var file = File.OpenRead("../../../Files/52191026665752000165550010000065651000361251.xml");

            var origem = (int)EOrigemDocumentoFiscal.UploadManual;

            var formData = new MultipartFormDataContent
            {
                { new StreamContent(file), "Arquivos", "52191026665752000165550010000065651000361251.xml" },
                { new StringContent(origem.ToString()), "Origem" },
                { new StringContent("Teste de Integração"), "Originador" }
            };

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsync($"{URL_BASE}/single", formData);

            // Assert
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Upload novo documento invalido")]
        [Trait("Upload", "Integração API - Single Inválido")]
        public async Task Upload_NovoArquivo_DeveRetornarComErro()
        {
            // Arrange
            var file = File.OpenRead("../../../Files/52191026665752000165550010000065651000361251.xml");

            var formData = new MultipartFormDataContent
            {
                { new StreamContent(file), "Arquivos", "52200104729121000120550010000226481000445597.xml" },
            };

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsync($"{URL_BASE}/single", formData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Upload vários documentos válidos"), TestPriority(1)]
        [Trait("Upload", "Integração API - Multiple Válidos")]
        public async Task Upload_VariosArquivos_DeveRetornarComSucesso()
        {
            // Arrange
            var fileTwo = File.OpenRead("../../../Files/52200100905760000300550030003642611114448182.xml");
            var fileThree = File.OpenRead("../../../Files/52200103346837000185550010000204331400697390.xml");
            var fileFour = File.OpenRead("../../../Files/52200104729121000120550010000226481000445597.xml");
            var fileFive = File.OpenRead("../../../Files/52200109074778000110550010000122421002673750.xml");
            var fileSix = File.OpenRead("../../../Files/52200109074778000110550010000122571002691670.xml");
            var fileSeven = File.OpenRead("../../../Files/52200126759727000140550010000003351660847978.xml");

            var origem = (int)EOrigemDocumentoFiscal.UploadManual;

            var formData = new MultipartFormDataContent
            {
                { new StreamContent(fileTwo), "Arquivos", "52200100905760000300550030003642611114448182.xml" },
                { new StreamContent(fileThree), "Arquivos", "52200103346837000185550010000204331400697390.xml" },
                { new StreamContent(fileFour), "Arquivos", "52200104729121000120550010000226481000445597.xml" },
                { new StreamContent(fileFive), "Arquivos", "52200109074778000110550010000122421002673750.xml" },
                { new StreamContent(fileSix), "Arquivos", "52200109074778000110550010000122571002691670.xml" },
                { new StreamContent(fileSeven), "Arquivos", "52200126759727000140550010000003351660847978.xml" },
                { new StringContent(origem.ToString()), "Origem" },
                { new StringContent("Teste de Integração"), "Originador" }
            };

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsync($"{URL_BASE}/multiple", formData);

            // Assert
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var retornoUpload = JsonConvert.DeserializeObject<RetornoUploadDTO>(await postResponse.Content.ReadAsStringAsync());

            Assert.NotNull(retornoUpload);
            Assert.Empty(retornoUpload.DocumentosNaoInseridos);
            Assert.NotEmpty(retornoUpload.DocumentosInseridos);
        }

        [Fact(DisplayName = "Upload vários documentos inválidos"), TestPriority(2)]
        [Trait("Upload", "Integração API - Multiple inválidos")]
        public async Task Upload_VariosArquivos_DeveRetornarComErro()
        {
            // Arrange
            var fileTwo = File.OpenRead("../../../Files/52200100905760000300550030003642611114448182.xml");
            var fileThree = File.OpenRead("../../../Files/52200103346837000185550010000204331400697390.xml");
            var fileFour = File.OpenRead("../../../Files/52200104729121000120550010000226481000445597.xml");
            var fileFive = File.OpenRead("../../../Files/52200109074778000110550010000122421002673750.xml");
            var fileSix = File.OpenRead("../../../Files/52200109074778000110550010000122571002691670.xml");
            var fileSeven = File.OpenRead("../../../Files/52200126759727000140550010000003351660847978.xml");

            var origem = (int)EOrigemDocumentoFiscal.UploadManual;

            var formData = new MultipartFormDataContent
            {
                { new StreamContent(fileTwo), "Arquivos", "52200100905760000300550030003642611114448182.xml" },
                { new StreamContent(fileThree), "Arquivos", "52200103346837000185550010000204331400697390.xml" },
                { new StreamContent(fileFour), "Arquivos", "52200104729121000120550010000226481000445597.xml" },
                { new StreamContent(fileFive), "Arquivos", "52200109074778000110550010000122421002673750.xml" },
                { new StreamContent(fileSix), "Arquivos", "52200109074778000110550010000122571002691670.xml" },
                { new StreamContent(fileSeven), "Arquivos", "52200126759727000140550010000003351660847978.xml" },
                { new StringContent(origem.ToString()), "Origem" },
                { new StringContent("Teste de Integração"), "Originador" }
            };

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsync($"{URL_BASE}/multiple", formData);

            // Assert
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var retornoUpload = JsonConvert.DeserializeObject<RetornoUploadDTO>(await postResponse.Content.ReadAsStringAsync());

            Assert.NotNull(retornoUpload);
            Assert.NotEmpty(retornoUpload.DocumentosNaoInseridos);
            Assert.Empty(retornoUpload.DocumentosInseridos);
        }
    }
}
