using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TronBox.API.Tests.Config;
using Xunit;

namespace TronBox.API.Tests
{
    [TestCaseOrderer("TronBox.API.Tests.Config.PriorityOrderer", "TronBox.API.Tests")]
    [Collection(nameof(IntegrationTestsFixtureCollection))]
    public class HistoricoConsultaMatoGrossoApiTests
    {
        internal const string URL_BASE = "api/v1/historicos-consulta-mato-grosso";
        private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

        public HistoricoConsultaMatoGrossoApiTests(IntegrationTestsFixture<StartupTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Inserir histórico de consulta válido")]
        [Trait("Histórico de Consulta Mato Grosso", "Integração API - Inserir")]
        public async Task HistoricoConsultaMatoGrosso_Novo_DeveRetornarComSucesso()
        {
            // Arrange
            var historicoConsultaMatoGrosso = _testsFixture.ObterHistoricoConsultaMatoGrossoValido();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, historicoConsultaMatoGrosso);

            // Assert
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Não inserir histórico inválido")]
        [Trait("Histórico de Consulta Mato Grosso", "Integração API - Inserir")]
        public async Task HistoricoConsultaMatoGrosso_NovoInvalido_DeveRetornarComErro()
        {
            // Arrange
            var historicoConsultaMatoGrosso = _testsFixture.ObterHistoricoConsultaMatoGrossoInvalido();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, historicoConsultaMatoGrosso);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }
    }
}
