namespace TronBox.Domain.DTO
{
    public class EstatisticaDTO
    {
        public string Id { get; set; }
        public long DataHora { get; set; }
        public bool CertificadoAtivo { get; set; }
        public int NotaFiscalEntrada { get; set; }
        public int NotaFiscalSaida { get; set; }
        public int NotaFiscalConsumidor { get; set; }
        public int NotaFiscalServicoEntrada { get; set; }
        public int NotaFiscalServicoSaida { get; set; }
        public int ConhecimentoTransporteEntrada { get; set; }
        public int ConhecimentoTransporteSaida { get; set; }
        public int ConhecimentoTransporteNaoTomador { get; set; }
        public int OrigemEmail { get; set; }
        public int OrigemUploadManual { get; set; }
        public int OrigemDownloadAgente { get; set; }
        public int OrigemAgenteManifestacao { get; set; }
        public int OrigemMonitorA3 { get; set; }
        public int OrigemPortalEstadual { get; set; }
        public int OrigemMonitorSincronizacao { get; set; }

        public EstatisticaDTO()
        {
        }

        public EstatisticaDTO(long dataHora, bool certificadoAtivo, int notaFiscalEntrada, int notaFiscalSaida, int notaFiscalConsumidor, int notaFiscalServicoEntrada, int notaFiscalServicoSaida, int conhecimentoTransporteEntrada, int conhecimentoTransporteSaida, int conhecimentoTransporteNaoTomador, int origemEmail, int origemUploadManual, int origemDownloadAgente, int origemAgenteManifestacao, int origemMonitorA3, int origemPortalEstadual, int origemMonitorSincronizacao)
        {
            DataHora = dataHora;
            CertificadoAtivo = certificadoAtivo;
            NotaFiscalEntrada = notaFiscalEntrada;
            NotaFiscalSaida = notaFiscalSaida;
            NotaFiscalConsumidor = notaFiscalConsumidor;
            NotaFiscalServicoEntrada = notaFiscalServicoEntrada;
            NotaFiscalServicoSaida = notaFiscalServicoSaida;
            ConhecimentoTransporteEntrada = conhecimentoTransporteEntrada;
            ConhecimentoTransporteSaida = conhecimentoTransporteSaida;
            ConhecimentoTransporteNaoTomador = conhecimentoTransporteNaoTomador;
            OrigemEmail = origemEmail;
            OrigemUploadManual = origemUploadManual;
            OrigemDownloadAgente = origemDownloadAgente;
            OrigemAgenteManifestacao = origemAgenteManifestacao;
            OrigemMonitorA3 = origemMonitorA3;
            OrigemPortalEstadual = origemPortalEstadual;
            OrigemMonitorSincronizacao = origemMonitorSincronizacao;
        }
    }
}
