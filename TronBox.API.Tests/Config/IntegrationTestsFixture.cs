using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TronBox.Domain.DTO;
using TronBox.Domain.DTO.InnerClassDTO;
using TronBox.Domain.Enums;
using TronCore.DefinicoesConfiguracoes;
using TronCore.Enumeradores;
using TronCore.Utilitarios;
using Xunit;

namespace TronBox.API.Tests.Config
{
    [CollectionDefinition(nameof(IntegrationTestsFixtureCollection))]
    public class IntegrationTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture<StartupTests>> { }

    public class LoginDTO
    {
        public string Token { get; set; }
        public long Expires { get; set; }
        public List<TenantLoginDTO> Tenants { get; set;}
    }

    public class TenantLoginDTO
    {
        public string TenantId { get; set; }
        public string Endereco { get; set; }
        public eModuloTron Modulo { get; set; }
    }

    public class IntegrationTestsFixture<TStartup> : IDisposable where TStartup : class
    {
        public readonly TronBoxAppFactory<TStartup> Factory;
        public HttpClient Client;
        public HttpClient ClientExternal;

        public string UsuarioToken;
        public string Tenant;

        internal const string userName = "contador.inovacao@tron.com.br";
        internal const string password = "123456";
        internal const string cnpj = "06006848000104";

        public IntegrationTestsFixture()
        {
            Factory = new TronBoxAppFactory<TStartup>();
            Client = Factory.CreateClient();

            ClientExternal = new HttpClient()
            {
                BaseAddress = new Uri(Constantes.URI_BASE_ST)
            };
        }

        public async Task ObterDadosAcesso()
        {
            var userData = new Credenciais
            {
                Usuario = userName,
                Senha = password
            };

            using (var response = await ClientExternal.PostAsJsonAsync("api/autenticacao/login", userData))
            {
                response.EnsureSuccessStatusCode();

                var dadosAcesso = JsonConvert.DeserializeObject<LoginDTO>(await response.Content.ReadAsStringAsync());
                var tenants = dadosAcesso.Tenants.Where(t => t.Modulo == eModuloTron.BX);

                Assert.NotEmpty(tenants);

                UsuarioToken = dadosAcesso.Token;
                Tenant = tenants.FirstOrDefault(c => c.Endereco == cnpj).TenantId;
            }
        }

        public ManifestoDTO ObterManifesto(ESituacaoManifesto situacaoManifesto)
        {
            switch (situacaoManifesto)
            {
                case ESituacaoManifesto.SemManifesto:
                    return new ManifestoDTO
                    {
                        ChaveDocumentoFiscal = "52200126759727000140550010000003351660847978",
                        NumeroDocumentoFiscal = "335",
                        ValorDocumentoFiscal = 4052.4,
                        DataArmazenamento = UtilitarioDatas.ConvertToIntDate(DateTime.Now),
                        DataEmissaoManifesto = 20200102,
                        DataManifesto = 20200102,
                        SituacaoManifesto = ESituacaoManifesto.SemManifesto,
                        SituacaoDocumentoFiscal = ESituacaoDocumentoFiscal.NaoArmazenado,
                        DadosOrigem = new DadosOrigemManifestoDTO
                        {
                            Origem = EOrigemManifesto.Agente,
                            Originador = "CONTAINER - WS NFe"
                        },
                        DadosFornecedor = new DadosFornecedorDTO
                        {
                            Inscricao = "26759727000140",
                            RazaoSocial = "SAMUEL O. ROCHA - JS COMERCIAL EIRELI - ME"
                        }
                    };
                case ESituacaoManifesto.Ciencia:
                    return new ManifestoDTO
                    {
                        ChaveDocumentoFiscal = "52200109074778000110550010000122421002673750",
                        NumeroDocumentoFiscal = "12242",
                        ValorDocumentoFiscal = 80.0,
                        DataArmazenamento = UtilitarioDatas.ConvertToIntDate(DateTime.Now),
                        DataEmissaoManifesto = 20200105,
                        DataManifesto = 20200105,
                        SituacaoManifesto = ESituacaoManifesto.Ciencia,
                        SituacaoDocumentoFiscal = ESituacaoDocumentoFiscal.NaoArmazenado,
                        DadosOrigem = new DadosOrigemManifestoDTO
                        {
                            Origem = EOrigemManifesto.Agente,
                            Originador = "CONTAINER - WS NFe"
                        },
                        DadosFornecedor = new DadosFornecedorDTO
                        {
                            Inscricao = "09074778000110",
                            RazaoSocial = "Petroleo Transbrasiliana Com Var de Comb Ltda"
                        }
                    };
                case ESituacaoManifesto.Confirmado:
                    return new ManifestoDTO
                    {
                        ChaveDocumentoFiscal = "52200109074778000110550010000122571002691670",
                        NumeroDocumentoFiscal = "12257",
                        ValorDocumentoFiscal = 80.0,
                        DataArmazenamento = UtilitarioDatas.ConvertToIntDate(DateTime.Now),
                        DataEmissaoManifesto = 20200107,
                        DataManifesto = 20200107,
                        SituacaoManifesto = ESituacaoManifesto.Confirmado,
                        SituacaoDocumentoFiscal = ESituacaoDocumentoFiscal.NaoArmazenado,
                        DadosOrigem = new DadosOrigemManifestoDTO
                        {
                            Origem = EOrigemManifesto.Agente,
                            Originador = "CONTAINER - WS NFe"
                        },
                        DadosFornecedor = new DadosFornecedorDTO
                        {
                            Inscricao = "09074778000110",
                            RazaoSocial = "Petroleo Transbrasiliana Com Var de Comb Ltda"
                        }
                    };
                case ESituacaoManifesto.Desconhecido:
                    return new ManifestoDTO
                    {
                        ChaveDocumentoFiscal = "52200103346837000185550010000204331400697390",
                        NumeroDocumentoFiscal = "20433",
                        ValorDocumentoFiscal = 1808.0,
                        DataArmazenamento = UtilitarioDatas.ConvertToIntDate(DateTime.Now),
                        DataEmissaoManifesto = 20200108,
                        DataManifesto = 20200108,
                        SituacaoManifesto = ESituacaoManifesto.Desconhecido,
                        SituacaoDocumentoFiscal = ESituacaoDocumentoFiscal.NaoArmazenado,
                        DadosOrigem = new DadosOrigemManifestoDTO
                        {
                            Origem = EOrigemManifesto.Agente,
                            Originador = "CONTAINER - WS NFe"
                        },
                        DadosFornecedor = new DadosFornecedorDTO
                        {
                            Inscricao = "03346837000185",
                            RazaoSocial = "TC TECNOLOGIA E INFORMATICA LTDA"
                        }
                    };
                case ESituacaoManifesto.NaoRealizado:
                    return new ManifestoDTO
                    {
                        ChaveDocumentoFiscal = "52200100905760000300550030003642611114448182",
                        NumeroDocumentoFiscal = "364261",
                        ValorDocumentoFiscal = 548.0,
                        DataArmazenamento = UtilitarioDatas.ConvertToIntDate(DateTime.Now),
                        DataEmissaoManifesto = 20200109,
                        DataManifesto = 20200109,
                        SituacaoManifesto = ESituacaoManifesto.NaoRealizado,
                        SituacaoDocumentoFiscal = ESituacaoDocumentoFiscal.NaoArmazenado,
                        DadosOrigem = new DadosOrigemManifestoDTO
                        {
                            Origem = EOrigemManifesto.Agente,
                            Originador = "CONTAINER - WS NFe"
                        },
                        DadosFornecedor = new DadosFornecedorDTO
                        {
                            Inscricao = "00905760000300",
                            RazaoSocial = "PAPELARIA TRIBUTARIA LTDA"
                        }
                    };
                case ESituacaoManifesto.CienciaAutomatica:
                    return new ManifestoDTO
                    {
                        ChaveDocumentoFiscal = "52200104729121000120550010000226481000445597",
                        NumeroDocumentoFiscal = "22648",
                        ValorDocumentoFiscal = 525.0,
                        DataArmazenamento = UtilitarioDatas.ConvertToIntDate(DateTime.Now),
                        DataEmissaoManifesto = 20200109,
                        DataManifesto = 20200109,
                        SituacaoManifesto = ESituacaoManifesto.CienciaAutomatica,
                        SituacaoDocumentoFiscal = ESituacaoDocumentoFiscal.NaoArmazenado,
                        DadosOrigem = new DadosOrigemManifestoDTO
                        {
                            Origem = EOrigemManifesto.Agente,
                            Originador = "CONTAINER - WS NFe"
                        },
                        DadosFornecedor = new DadosFornecedorDTO
                        {
                            Inscricao = "04729121000120",
                            RazaoSocial = "COMPUSAT INFORMATICA LTDA"
                        }
                    };
                case ESituacaoManifesto.Cancelado:
                    return new ManifestoDTO
                    {
                        ChaveDocumentoFiscal = "52191026665752000165550010000065651000361251",
                        NumeroDocumentoFiscal = "6565",
                        ValorDocumentoFiscal = 22.0 ,
                        DataArmazenamento = UtilitarioDatas.ConvertToIntDate(DateTime.Now),
                        DataEmissaoManifesto = 20191030,
                        DataManifesto = 20191030,
                        SituacaoManifesto = ESituacaoManifesto.Cancelado,
                        SituacaoDocumentoFiscal = ESituacaoDocumentoFiscal.NaoArmazenado,
                        DadosOrigem = new DadosOrigemManifestoDTO
                        {
                            Origem = EOrigemManifesto.Agente,
                            Originador = "CONTAINER - WS NFe"
                        },
                        DadosFornecedor = new DadosFornecedorDTO
                        {
                            Inscricao = "26665752000165",
                            RazaoSocial = "EMBALAGENS BOM SUCESSO LTDA"
                        }
                    };
                default:
                    return null;
            }
        }

        public ManifestoDTO ObterManifestoInvalido()
        {
            return new ManifestoDTO
            {
                NumeroDocumentoFiscal = "335",
                ValorDocumentoFiscal = 4052.4,
                DataEmissaoManifesto = 20200102,
                DataManifesto = 20200102,
                SituacaoManifesto = ESituacaoManifesto.SemManifesto,
                DadosOrigem = new DadosOrigemManifestoDTO
                {
                    Origem = EOrigemManifesto.Agente,
                    Originador = "CONTAINER - WS NFe"
                },
                DadosFornecedor = new DadosFornecedorDTO
                {
                    Inscricao = "26759727000140",
                    RazaoSocial = "SAMUEL O. ROCHA - JS COMERCIAL EIRELI - ME"
                }
            };
        }

        public HistoricoConsultaDTO ObterHistoricoConsultaValido()
        {
            return new HistoricoConsultaDTO
            {
                TipoDocumentoConsulta = new Faker().PickRandom<ETipoDocumentoConsulta>(),
                TipoConsulta = new Faker().PickRandom<ETipoConsulta>(),
                DataHoraConsulta = UtilitarioDatas.ConvertToIntDateTime(DateTime.Now),
                UltimoNSU = new Faker().Random.AlphaNumeric(10),
                DocumentosEncontrados = new Faker().Random.Int(),
                DocumentosArmazenados = new Faker().Random.Int()
            };
        }

        public HistoricoConsultaDTO ObterHistoricoConsultaInvalido() => new HistoricoConsultaDTO();

        public HistoricoConsultaMatoGrossoDTO ObterHistoricoConsultaMatoGrossoValido()
        {
            return new HistoricoConsultaMatoGrossoDTO
            {
                InscricaoEstadual = new Faker().Random.AlphaNumeric(10),
                TipoConsulta = new Faker().PickRandom<ETipoConsulta>(),
                DataHoraConsulta = UtilitarioDatas.ConvertToIntDateTime(DateTime.Now),
                DataInicialConsultada = UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddDays(-30)),
                DataFinalConsultada = UtilitarioDatas.ConvertToIntDate(DateTime.Now),
                ChavesEncontradas = new Faker().Make(3, () => new Faker().Random.AlphaNumeric(44))
            };
        }

        public HistoricoConsultaMatoGrossoDTO ObterHistoricoConsultaMatoGrossoInvalido() => new HistoricoConsultaMatoGrossoDTO();

        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
        }
    }
}
