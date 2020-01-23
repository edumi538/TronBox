using Bogus;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TronBox.API.Tests.Config;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using TronCore.Utilitarios;
using Xunit;

namespace TronBox.API.Tests
{
    [TestCaseOrderer("TronBox.API.Tests.Config.PriorityOrderer", "TronBox.API.Tests")]
    [Collection(nameof(IntegrationTestsFixtureCollection))]
    public class ManifestoApiTests
    {
        private readonly IntegrationTestsFixture<StartupTests> _testsFixture;
        internal const string URL_BASE = "api/v1/manifestos";
        internal const string FILTRO_SITUACAO = "{'campo': 'situacaoManifesto', 'valor': SITUACAO }";
        internal const string FILTRO_DATA_INICIAL = "{'campo': 'dataEmissaoManifesto', 'valor': DATAINICIAL, 'condicao': 'gte' }";
        internal const string FILTRO_DATA_FINAL = "{'campo': 'dataEmissaoManifesto', 'valor': DATAFINAL, 'condicao': 'lte' }";

        public ManifestoApiTests(IntegrationTestsFixture<StartupTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Nã inserir novo manifesto invalido")]
        [Trait("Manifesto", "Integração API - Inserir")]
        public async Task Manifesto_NovoManifestoInvalido_DeveRetornarComErro()
        {
            // Arrange
            var manifesto = _testsFixture.ObterManifestoInvalido();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, manifesto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Inserir novo manifesto sem manifestação"), TestPriority(1)]
        [Trait("Manifesto", "Integração API - Inserir")]
        public async Task Manifesto_NovoManifestoSemManifestacao_DeveRetornarComSucesso()
        {
            // Arrange
            var manifesto = _testsFixture.ObterManifesto(ESituacaoManifesto.SemManifesto);

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, manifesto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Inserir novo manifesto com ciência"), TestPriority(2)]
        [Trait("Manifesto", "Integração API - Inserir")]
        public async Task Manifesto_NovoManifestoComCiencia_DeveRetornarComSucesso()
        {
            // Arrange
            var manifesto = _testsFixture.ObterManifesto(ESituacaoManifesto.Ciencia);

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, manifesto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Inserir novo manifesto confirmado"), TestPriority(3)]
        [Trait("Manifesto", "Integração API - Inserir")]
        public async Task Manifesto_NovoManifestoConfirmado_DeveRetornarComSucesso()
        {
            // Arrange
            var manifesto = _testsFixture.ObterManifesto(ESituacaoManifesto.Confirmado);

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, manifesto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Inserir novo manifesto desconhecido"), TestPriority(4)]
        [Trait("Manifesto", "Integração API - Inserir")]
        public async Task Manifesto_NovoManifestoDesconhecido_DeveRetornarComSucesso()
        {
            // Arrange
            var manifesto = _testsFixture.ObterManifesto(ESituacaoManifesto.Desconhecido);

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, manifesto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Inserir novo manifesto não realizado"), TestPriority(5)]
        [Trait("Manifesto", "Integração API - Inserir")]
        public async Task Manifesto_NovoManifestoNaoRealizado_DeveRetornarComSucesso()
        {
            // Arrange
            var manifesto = _testsFixture.ObterManifesto(ESituacaoManifesto.NaoRealizado);

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, manifesto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Inserir novo manifesto ciência automatica"), TestPriority(6)]
        [Trait("Manifesto", "Integração API - Inserir")]
        public async Task Manifesto_NovoManifestoCienciaAutomatica_DeveRetornarComSucesso()
        {
            // Arrange
            var manifesto = _testsFixture.ObterManifesto(ESituacaoManifesto.CienciaAutomatica);

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, manifesto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Inserir novo manifesto cancelado"), TestPriority(7)]
        [Trait("Manifesto", "Integração API - Inserir")]
        public async Task Manifesto_NovoManifestoCancelado_DeveRetornarComSucesso()
        {
            // Arrange
            var manifesto = _testsFixture.ObterManifesto(ESituacaoManifesto.Cancelado);

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, manifesto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Bloquear manifesto existente"), TestPriority(8)]
        [Trait("Manifesto", "Integração API - Inserir")]
        public async Task Manifesto_NovoManifesto_DeveRetornarErro()
        {
            // Arrange
            var manifesto = _testsFixture.ObterManifesto(new Faker().PickRandom<ESituacaoManifesto>());

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var postResponse = await _testsFixture.Client.PostAsJsonAsync(URL_BASE, manifesto);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, postResponse.StatusCode);
        }

        [Fact(DisplayName = "Obter manifestos sem manifestação"), TestPriority(9)]
        [Trait("Manifesto", "Integração API - Buscar")]
        public async Task Manifesto_ObterManifestos_DeveRetornoApenasSemManifestacao()
        {
            // Arrange
            var situacao = ((int)ESituacaoManifesto.SemManifesto).ToString();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var getResponse = await _testsFixture.Client.GetAsync($"{URL_BASE}?filtro=[{FILTRO_SITUACAO.Replace("SITUACAO", situacao)}]");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var manifestos = JsonConvert.DeserializeObject<List<ManifestoDTO>>(await getResponse.Content.ReadAsStringAsync());

            Assert.NotNull(manifestos);
            Assert.NotEmpty(manifestos);
            Assert.Empty(manifestos.Where(c => c.SituacaoManifesto != ESituacaoManifesto.SemManifesto));
        }

        [Fact(DisplayName = "Obter manifestos confirmados"), TestPriority(10)]
        [Trait("Manifesto", "Integração API - Buscar")]
        public async Task Manifesto_ObterManifestos_DeveRetornoApenasConfirmado()
        {
            // Arrange
            var situacao = ((int)ESituacaoManifesto.Confirmado).ToString();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var getResponse = await _testsFixture.Client.GetAsync($"{URL_BASE}?filtro=[{FILTRO_SITUACAO.Replace("SITUACAO", situacao)}]");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var manifestos = JsonConvert.DeserializeObject<List<ManifestoDTO>>(await getResponse.Content.ReadAsStringAsync());

            Assert.NotNull(manifestos);
            Assert.NotEmpty(manifestos);
            Assert.Empty(manifestos.Where(c => c.SituacaoManifesto != ESituacaoManifesto.Confirmado));
        }

        [Fact(DisplayName = "Obter manifestos com ciencia"), TestPriority(11)]
        [Trait("Manifesto", "Integração API - Buscar")]
        public async Task Manifesto_ObterManifestos_DeveRetornoApenasCiencia()
        {
            // Arrange
            var situacao = ((int)ESituacaoManifesto.Ciencia).ToString();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var getResponse = await _testsFixture.Client.GetAsync($"{URL_BASE}?filtro=[{FILTRO_SITUACAO.Replace("SITUACAO", situacao)}]");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var manifestos = JsonConvert.DeserializeObject<List<ManifestoDTO>>(await getResponse.Content.ReadAsStringAsync());

            Assert.NotNull(manifestos);
            Assert.NotEmpty(manifestos);
            Assert.Empty(manifestos.Where(c => c.SituacaoManifesto != ESituacaoManifesto.Ciencia));
        }

        [Fact(DisplayName = "Obter manifestos desconhecido"), TestPriority(12)]
        [Trait("Manifesto", "Integração API - Inserir")]
        public async Task Manifesto_ObterManifestos_DeveRetornoApenasDesconhecido()
        {
            // Arrange
            var situacao = ((int)ESituacaoManifesto.Desconhecido).ToString();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var getResponse = await _testsFixture.Client.GetAsync($"{URL_BASE}?filtro=[{FILTRO_SITUACAO.Replace("SITUACAO", situacao)}]");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var manifestos = JsonConvert.DeserializeObject<List<ManifestoDTO>>(await getResponse.Content.ReadAsStringAsync());

            Assert.NotNull(manifestos);
            Assert.NotEmpty(manifestos);
            Assert.Empty(manifestos.Where(c => c.SituacaoManifesto != ESituacaoManifesto.Desconhecido));
        }

        [Fact(DisplayName = "Obter manifestos não realizado"), TestPriority(13)]
        [Trait("Manifesto", "Integração API - Buscar")]
        public async Task Manifesto_ObterManifestos_DeveRetornoApenasNaoRealizado()
        {
            // Arrange
            var situacao = ((int)ESituacaoManifesto.NaoRealizado).ToString();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var getResponse = await _testsFixture.Client.GetAsync($"{URL_BASE}?filtro=[{FILTRO_SITUACAO.Replace("SITUACAO", situacao)}]");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var manifestos = JsonConvert.DeserializeObject<List<ManifestoDTO>>(await getResponse.Content.ReadAsStringAsync());

            Assert.NotNull(manifestos);
            Assert.NotEmpty(manifestos);
            Assert.Empty(manifestos.Where(c => c.SituacaoManifesto != ESituacaoManifesto.NaoRealizado));
        }

        [Fact(DisplayName = "Obter manifestos com ciencia automatica"), TestPriority(14)]
        [Trait("Manifesto", "Integração API - Buscar")]
        public async Task Manifesto_ObterManifestos_DeveRetornoApenasCienciaAutomatica()
        {
            // Arrange
            var situacao = ((int)ESituacaoManifesto.CienciaAutomatica).ToString();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var getResponse = await _testsFixture.Client.GetAsync($"{URL_BASE}?filtro=[{FILTRO_SITUACAO.Replace("SITUACAO", situacao)}]");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var manifestos = JsonConvert.DeserializeObject<List<ManifestoDTO>>(await getResponse.Content.ReadAsStringAsync());

            Assert.NotNull(manifestos);
            Assert.NotEmpty(manifestos);
            Assert.Empty(manifestos.Where(c => c.SituacaoManifesto != ESituacaoManifesto.CienciaAutomatica));
        }

        [Fact(DisplayName = "Obter manifestos cancelado"), TestPriority(15)]
        [Trait("Manifesto", "Integração API - Buscar")]
        public async Task Manifesto_ObterManifestos_DeveRetornoApenasCancelado()
        {
            // Arrange
            var situacao = ((int)ESituacaoManifesto.Cancelado).ToString();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            // Act
            var getResponse = await _testsFixture.Client.GetAsync($"{URL_BASE}?filtro=[{FILTRO_SITUACAO.Replace("SITUACAO", situacao)}]");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var manifestos = JsonConvert.DeserializeObject<List<ManifestoDTO>>(await getResponse.Content.ReadAsStringAsync());

            Assert.NotNull(manifestos);
            Assert.NotEmpty(manifestos);
            Assert.Empty(manifestos.Where(c => c.SituacaoManifesto != ESituacaoManifesto.Cancelado));
        }

        [Fact(DisplayName = "Deletar manifesto"), TestPriority(16)]
        [Trait("Manifesto", "Integração API - Deletar")]
        public async Task Manifesto_Deletar_DeveRetornarSucesso()
        {
            // Arrange
            var situacao = ((int)ESituacaoManifesto.Cancelado).ToString();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            var getResponse = await _testsFixture.Client.GetAsync($"{URL_BASE}?filtro=[{FILTRO_SITUACAO.Replace("SITUACAO", situacao)}]");

            var manifesto = JsonConvert.DeserializeObject<List<ManifestoDTO>>(await getResponse.Content.ReadAsStringAsync()).FirstOrDefault();

            // Act
            var deleteResponse = await _testsFixture.Client.DeleteAsync($"{URL_BASE}/{manifesto.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        [Fact(DisplayName = "Atualizar de Sem Manifesto para Ciencia"), TestPriority(17)]
        [Trait("Manifesto", "Integração API - Atualizar")]
        public async Task Manifesto_AtualizarSituacaoManifesto_DeveRetornarSucesso()
        {
            // Arrange
            var situacao = ((int)ESituacaoManifesto.SemManifesto).ToString();

            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            var getResponse = await _testsFixture.Client.GetAsync($"{URL_BASE}?filtro=[{FILTRO_SITUACAO.Replace("SITUACAO", situacao)}]");

            var manifesto = JsonConvert.DeserializeObject<List<ManifestoDTO>>(await getResponse.Content.ReadAsStringAsync()).FirstOrDefault();

            var patchManifesto = new { SituacaoManifesto = ESituacaoManifesto.Ciencia };

            // Act
            var patchResponse = await _testsFixture.Client.PatchAsJsonAsync($"{URL_BASE}/{manifesto.Id}", patchManifesto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);
        }

        [Fact(DisplayName = "Obter manifestos entre duas datas"), TestPriority(18)]
        [Trait("Manifesto", "Integração API - Buscar")]
        public async Task Manifesto_ObterManifestosEntreDuasDatas_DeveRetornoComSucesso()
        {
            // Arrange
            await _testsFixture.ObterDadosAcesso();
            _testsFixture.Client.AtribuirHeaders(_testsFixture.UsuarioToken, _testsFixture.Tenant);

            var dataInicial = "20200101";
            var dataFinal = "20200131";

            // Act
            var getResponse = await _testsFixture.Client.GetAsync(
                $"{URL_BASE}?filtro=[{FILTRO_DATA_INICIAL.Replace("DATAINICIAL", dataInicial)}, {FILTRO_DATA_FINAL.Replace("DATAFINAL", dataFinal)}]");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var manifestos = JsonConvert.DeserializeObject<List<ManifestoDTO>>(await getResponse.Content.ReadAsStringAsync());

            Assert.NotNull(manifestos);
            Assert.NotEmpty(manifestos);
            Assert.Empty(manifestos.Where(c => c.DataEmissaoManifesto <= 20200101 || c.DataEmissaoManifesto >= 20200131));
        }
    }
}
