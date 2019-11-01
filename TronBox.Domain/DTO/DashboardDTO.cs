using System;
using System.Globalization;
using TronBox.Domain.Enums;
using TronCore.Enumeradores.Helpers;

namespace TronBox.Domain.DTO
{
    public class DashboardDocumentosDTO
    {
        public ETipoDocumentoFiscal Tipo { get; set; }
        public int Quantidade { get; set; }
        public string DescricaoTipo
        {
            get => EnumHelper<ETipoDocumentoFiscal>.GetDisplayValue(Tipo);
        }
    }

    public class DashboardOrigemDocumentoDTO
    {
        public EOrigemDocumentoFiscal Origem { get; set; }
        public int Quantidade { get; set; }
        public string DescricaoOrigem
        {
            get => EnumHelper<EOrigemDocumentoFiscal>.GetDisplayValue(Origem);
        }
    }

    public class DashboardUltimaSemanaDTO
    {
        public DayOfWeek DiaSemana { get; set; }
        public int NfeEntrada { get; set; }
        public int NfeSaida { get; set; }
        public int CteEntrada { get; set; }
        public int CteSaida { get; set; }
        public int Nfce { get; set; }
        public int NfseEntrada { get; set; }
        public int NfseSaida { get; set; }
        public string DescricaoDiaSemana
        {
            get => new CultureInfo("pt-BR").DateTimeFormat.GetDayName(DiaSemana);
        }
    }
}
