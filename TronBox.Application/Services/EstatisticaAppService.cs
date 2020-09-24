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

                var estatisticaDto = new EstatisticaDTO
                {
                    DataHora = dataHora,
                    CertificadoAtivo = certificadoAtivo,
                    NotaFiscalEntrada = documentosFiscais.Count(c => c.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfeEntrada),
                    NotaFiscalSaida = documentosFiscais.Count(c => c.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfeSaida),
                    NotaFiscalConsumidor = documentosFiscais.Count(c => c.TipoDocumentoFiscal == ETipoDocumentoFiscal.Nfce),
                    NotaFiscalServicoEntrada = documentosFiscais.Count(c => c.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseEntrada),
                    NotaFiscalServicoSaida = documentosFiscais.Count(c => c.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseSaida),
                    ConhecimentoTransporteEntrada = documentosFiscais.Count(c => c.TipoDocumentoFiscal == ETipoDocumentoFiscal.CteEntrada),
                    ConhecimentoTransporteSaida = documentosFiscais.Count(c => c.TipoDocumentoFiscal == ETipoDocumentoFiscal.CteSaida),
                    ConhecimentoTransporteNaoTomador = documentosFiscais.Count(c => c.TipoDocumentoFiscal == ETipoDocumentoFiscal.CTeNaoTomador),
                };

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

            if (certificado != null) return false;

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
