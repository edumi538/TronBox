using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg.Repository;
using TronBox.Domain.Aggregates.ManifestoAgg;
using TronBox.Domain.Aggregates.ManifestoAgg.Repository;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using TronCore.Dominio.Specifications;
using TronCore.Persistencia.Interfaces;

namespace TronBox.Application.Services
{
    public class DashboardAppService : IDashboardAppService
    {
        #region Membros
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;
        #endregion

        #region Construtor
        public DashboardAppService(IMapper mapper, IRepositoryFactory repositoryFactory)
        {
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
        }
        #endregion

        public void Dispose()
        {
        }

        public long ContarDocumentos() => _repositoryFactory.Instancie<IDocumentoFiscalRepository>().Contar(new DirectSpecification<DocumentoFiscal>(c => true));

        public long ContarSemManifesto() => _repositoryFactory.Instancie<IManifestoRepository>().Contar(new DirectSpecification<Manifesto>(c => c.SituacaoManifesto == ESituacaoManifesto.SemManifesto));

        public List<DashboardDocumentosDTO> ObterDadosDocumentosArmazenados(int dataInicial, int dataFinal)
        {
            var documentosFiscais = BuscarDocumentos(dataInicial, dataFinal);

            List<DashboardDocumentosDTO> dashboardDocumentos = InicializarDashboardDocumento();

            foreach (var documentoFiscal in documentosFiscais)
            {
                var dashboardDocumento = dashboardDocumentos.Where(c => c.Tipo == documentoFiscal.TipoDocumentoFiscal).SingleOrDefault();

                DashboardDocumentosDTO dd = dashboardDocumento;
                dd.Quantidade++;

                int idx = dashboardDocumentos.IndexOf(dashboardDocumento);

                dashboardDocumentos.Remove(dashboardDocumento);
                dashboardDocumentos.Insert(idx, dd);
            }

            return dashboardDocumentos;
        }

        public List<DashboardOrigemDocumentoDTO> ObterDadosOrigemDocumentos(int dataInicial, int dataFinal)
        {
            var documentosFiscais = BuscarDocumentos(dataInicial, dataFinal);

            List<DashboardOrigemDocumentoDTO> dashboardOrigemDocumentos = InicializarDashboardOrigemDocumento();

            foreach (var documentoFiscal in documentosFiscais)
            {
                var dashboardOrigemDocumento = dashboardOrigemDocumentos.Where(c => c.Origem == documentoFiscal.DadosOrigem.Origem).SingleOrDefault();

                DashboardOrigemDocumentoDTO dod = dashboardOrigemDocumento;
                dod.Quantidade++;

                int idx = dashboardOrigemDocumentos.IndexOf(dashboardOrigemDocumento);

                dashboardOrigemDocumentos.Remove(dashboardOrigemDocumento);
                dashboardOrigemDocumentos.Insert(idx, dod);
            }

            return dashboardOrigemDocumentos;
        }

        public List<DashboardUltimaSemanaDTO> ObterDadosUltimaSemana(int dataInicial, int dataFinal)
        {
            var documentosFiscais = BuscarDocumentos(dataInicial, dataFinal);

            List<DashboardUltimaSemanaDTO> dashboardUltimaSemana = InicializarDashboardUltimaSemana();

            foreach (var documentoFiscal in documentosFiscais)
            {
                var date = DateTime.ParseExact(documentoFiscal.DataEmissaoDocumento.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                var dashboardOrigemDocumento = dashboardUltimaSemana.Where(c => c.DiaSemana == date.DayOfWeek).SingleOrDefault();

                DashboardUltimaSemanaDTO dus = dashboardOrigemDocumento;

                switch (documentoFiscal.TipoDocumentoFiscal)
                {
                    case ETipoDocumentoFiscal.NfeEntrada:
                        dus.NfeEntrada++;
                        break;
                    case ETipoDocumentoFiscal.NfeSaida:
                        dus.NfeSaida++;
                        break;
                    case ETipoDocumentoFiscal.CteEntrada:
                        dus.CteEntrada++;
                        break;
                    case ETipoDocumentoFiscal.CteSaida:
                        dus.CteSaida++;
                        break;
                    case ETipoDocumentoFiscal.Nfce:
                        dus.Nfce++;
                        break;
                    case ETipoDocumentoFiscal.NfseEntrada:
                        dus.NfseEntrada++;
                        break;
                    case ETipoDocumentoFiscal.NfseSaida:
                        dus.NfseSaida++;
                        break;
                    default:
                        break;
                }

                int idx = dashboardUltimaSemana.IndexOf(dashboardOrigemDocumento);

                dashboardUltimaSemana.Remove(dashboardOrigemDocumento);
                dashboardUltimaSemana.Insert(idx, dus);
            }

            return dashboardUltimaSemana;
        }

        #region Private Methods
        private IEnumerable<DocumentoFiscalDTO> BuscarDocumentos(int dataInicial, int dataFinal)
        {
            Specification<DocumentoFiscal> spec = new DirectSpecification<DocumentoFiscal>(c => c.DataArmazenamento >= AdicionaHorasDataInt(dataInicial) && c.DataArmazenamento <= AdicionaHorasDataInt(dataFinal));

            return _mapper.Map<IEnumerable<DocumentoFiscalDTO>>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>().BuscarTodos(spec));
        }

        private static List<DashboardDocumentosDTO> InicializarDashboardDocumento()
        {
            return new List<DashboardDocumentosDTO>()
            {
                new DashboardDocumentosDTO() { Tipo = ETipoDocumentoFiscal.NfeEntrada, Quantidade = 0 },
                new DashboardDocumentosDTO() { Tipo = ETipoDocumentoFiscal.NfeSaida, Quantidade = 0 },
                new DashboardDocumentosDTO() { Tipo = ETipoDocumentoFiscal.CteEntrada, Quantidade = 0 },
                new DashboardDocumentosDTO() { Tipo = ETipoDocumentoFiscal.CteSaida, Quantidade = 0 },
                new DashboardDocumentosDTO() { Tipo = ETipoDocumentoFiscal.Nfce, Quantidade = 0 },
                new DashboardDocumentosDTO() { Tipo = ETipoDocumentoFiscal.NfseEntrada, Quantidade = 0 },
                new DashboardDocumentosDTO() { Tipo = ETipoDocumentoFiscal.NfseSaida, Quantidade = 0 },
            };
        }

        private static List<DashboardUltimaSemanaDTO> InicializarDashboardUltimaSemana()
        {
            return new List<DashboardUltimaSemanaDTO>()
            {
                new DashboardUltimaSemanaDTO() { DiaSemana = DayOfWeek.Monday },
                new DashboardUltimaSemanaDTO() { DiaSemana = DayOfWeek.Tuesday },
                new DashboardUltimaSemanaDTO() { DiaSemana = DayOfWeek.Wednesday },
                new DashboardUltimaSemanaDTO() { DiaSemana = DayOfWeek.Thursday },
                new DashboardUltimaSemanaDTO() { DiaSemana = DayOfWeek.Friday },
                new DashboardUltimaSemanaDTO() { DiaSemana = DayOfWeek.Saturday },
                new DashboardUltimaSemanaDTO() { DiaSemana = DayOfWeek.Sunday },
            };
        }

        private static List<DashboardOrigemDocumentoDTO> InicializarDashboardOrigemDocumento()
        {
            return new List<DashboardOrigemDocumentoDTO>()
            {
                new DashboardOrigemDocumentoDTO() { Origem = EOrigemDocumentoFiscal.AgenteManifestacao, Quantidade = 0 },
                new DashboardOrigemDocumentoDTO() { Origem = EOrigemDocumentoFiscal.DownloadAgente, Quantidade = 0 },
                new DashboardOrigemDocumentoDTO() { Origem = EOrigemDocumentoFiscal.PortalEstadual, Quantidade = 0 },
                new DashboardOrigemDocumentoDTO() { Origem = EOrigemDocumentoFiscal.Email, Quantidade = 0 },
                new DashboardOrigemDocumentoDTO() { Origem = EOrigemDocumentoFiscal.MonitorA3, Quantidade = 0 },
                new DashboardOrigemDocumentoDTO() { Origem = EOrigemDocumentoFiscal.UploadManual, Quantidade = 0 },
                new DashboardOrigemDocumentoDTO() { Origem = EOrigemDocumentoFiscal.MonitorSincronizacao, Quantidade = 0},
            };
        }

        private long AdicionaHorasDataInt(int data)
        {
            return Convert.ToInt64(data * Math.Pow(10, 4));
        }
        #endregion
    }
}
