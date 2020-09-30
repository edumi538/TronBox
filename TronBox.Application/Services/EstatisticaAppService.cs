using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using Comum.Domain.Interfaces;
using Comum.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg.Repository;
using TronBox.Domain.Aggregates.EstatisticaAgg;
using TronBox.Domain.Aggregates.EstatisticaAgg.Repository;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using TronCore.DefinicoesConfiguracoes;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class EstatisticaAppService : IEstatisticaAppService
    {
        #region Membros
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IPessoaUsuarioLogado _usuarioLogado;
        #endregion

        #region Construtor
        public EstatisticaAppService(IBus bus, IMapper mapper, IRepositoryFactory repositoryFactory, IPessoaUsuarioLogado usuarioLogado)
        {
            _bus = bus;
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
            _usuarioLogado = usuarioLogado;
        }
        #endregion

        public void Dispose()
        {
        }

        public IEnumerable<EstatisticaDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<EstatisticaDTO>>(_repositoryFactory.Instancie<IEstatisticaRepository>()
            .BuscarTodos(new UtilitarioSpecification<Estatistica>().CriarEspecificacaoFiltro(filtro)).OrderByDescending(c => c.DataHora));

        public async Task<EstatisticaDTO> Calcular(long dataHora)
        {
            var estatisticaExistente = _mapper.Map<EstatisticaDTO>(_repositoryFactory.Instancie<IEstatisticaRepository>().BuscarPorExpressao(c => c.DataHora == dataHora));

            if (estatisticaExistente == null)
            {
                var certificadoAtivo = await EmpresaTemCertificadoAtivo();

                var documentosFiscais = _repositoryFactory.Instancie<IDocumentoFiscalRepository>().BuscarTodos();

                #region Iniciando Variaveis
                var notaFiscalEntrada = 0;
                var notaFiscalSaida = 0;
                var notaFiscalConsumidor = 0;
                var notaFiscalServicoEntrada = 0;
                var notaFiscalServicoSaida = 0;
                var conhecimentoTransporteEntrada = 0;
                var conhecimentoTransporteSaida = 0;
                var conhecimentoTransporteNaoTomador = 0;
                var origemEmail = 0;
                var origemUploadManual = 0;
                var origemDownloadAgente = 0;
                var origemAgenteManifestacao = 0;
                var origemMonitorA3 = 0;
                var origemPortalEstadual = 0;
                var origemMonitorSincronizacao = 0;
                #endregion

                #region Populando Variaveis
                using (var enumerator = documentosFiscais.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var documentoFiscal = enumerator.Current;

                        switch (documentoFiscal.TipoDocumentoFiscal)
                        {
                            case ETipoDocumentoFiscal.NfeEntrada:
                                notaFiscalEntrada += 1;
                                break;
                            case ETipoDocumentoFiscal.NfeSaida:
                                notaFiscalSaida += 1;
                                break;
                            case ETipoDocumentoFiscal.CteEntrada:
                                conhecimentoTransporteEntrada += 1;
                                break;
                            case ETipoDocumentoFiscal.CteSaida:
                                conhecimentoTransporteSaida += 1;
                                break;
                            case ETipoDocumentoFiscal.Nfce:
                                notaFiscalConsumidor += 1;
                                break;
                            case ETipoDocumentoFiscal.NfseEntrada:
                                notaFiscalServicoEntrada += 1;
                                break;
                            case ETipoDocumentoFiscal.NfseSaida:
                                notaFiscalServicoSaida += 1;
                                break;
                            case ETipoDocumentoFiscal.CTeNaoTomador:
                                conhecimentoTransporteNaoTomador += 1;
                                break;
                        }

                        switch (documentoFiscal.DadosOrigem.Origem)
                        {
                            case EOrigemDocumentoFiscal.Email:
                                origemEmail += 1;
                                break;
                            case EOrigemDocumentoFiscal.UploadManual:
                                origemUploadManual += 1;
                                break;
                            case EOrigemDocumentoFiscal.DownloadAgente:
                                origemDownloadAgente += 1;
                                break;
                            case EOrigemDocumentoFiscal.AgenteManifestacao:
                                origemAgenteManifestacao += 1;
                                break;
                            case EOrigemDocumentoFiscal.MonitorA3:
                                origemMonitorA3 += 1;
                                break;
                            case EOrigemDocumentoFiscal.PortalEstadual:
                                origemPortalEstadual += 1;
                                break;
                            case EOrigemDocumentoFiscal.MonitorSincronizacao:
                                origemMonitorSincronizacao += 1;
                                break;
                        }
                    }
                }
                #endregion

                var estatisticaDto = new EstatisticaDTO(dataHora, certificadoAtivo, notaFiscalEntrada, notaFiscalSaida, notaFiscalConsumidor,
                    notaFiscalServicoEntrada, notaFiscalServicoSaida, conhecimentoTransporteEntrada, conhecimentoTransporteSaida,
                    conhecimentoTransporteNaoTomador, origemEmail, origemUploadManual, origemDownloadAgente, origemAgenteManifestacao,
                    origemMonitorA3, origemPortalEstadual, origemMonitorSincronizacao);

                var estatistica = _mapper.Map<Estatistica>(estatisticaDto);

                if (estatistica.EhValido())
                {
                    _repositoryFactory.Instancie<IEstatisticaRepository>().Inserir(estatistica);

                    return estatisticaDto;
                }

                GerarErros(estatistica);

                return null;
            }

            return estatisticaExistente;
        }

        private async Task<bool> EmpresaTemCertificadoAtivo()
        {
            var empresa = _mapper.Map<EmpresaDTO>(_repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault());

            var certificado = await UtilitarioHttpClient.GetRequest(_usuarioLogado.GetToken(), Constantes.URI_BASE_CT,
                $"api/v1/certificados/{empresa.Inscricao}");

            if (certificado == null) return false;

            var dadosCertificado = JsonConvert.DeserializeObject<CertificadoSimplificadoDTO>(certificado);

            return dadosCertificado.DataVencimento <= UtilitarioDatas.ConvertToIntDateTime(DateTime.Now);
        }

        private void GerarErros(Estatistica Estatistica)
        {
            foreach (var error in Estatistica.ValidationResult.Errors)
                _bus.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));
        }
    }
}
