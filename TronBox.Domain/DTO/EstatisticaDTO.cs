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
    }
}
