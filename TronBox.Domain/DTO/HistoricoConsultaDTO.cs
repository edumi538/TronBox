using System;
using TronBox.Domain.Enums;
using TronCore.Enumeradores.Helpers;
using TronCore.Utilitarios;

namespace TronBox.Domain.DTO
{
    public class HistoricoConsultaDTO
    {
        public string Id { get; set; }
        public ETipoDocumentoConsulta TipoDocumentoConsulta { get; set; }
        public ETipoConsulta TipoConsulta { get; set; }
        public long DataHoraConsulta { get; set; }
        public string UltimoNSU { get; set; }
        public int DocumentosEncontrados { get; set; }
        public int DocumentosArmazenados { get; set; }
        public string DescricaoTipoConsulta
        {
            get => EnumHelper<ETipoConsulta>.GetDisplayValue(TipoConsulta);

        }
        public string DescricaoTipoDocumentoConsulta
        {
            get => EnumHelper<ETipoDocumentoConsulta>.GetDisplayValue(TipoDocumentoConsulta);
        }
        public DateTime DataHoraConsultaFormatada
        {
            get => UtilitarioDatas.ConvertIntToDateTime(DataHoraConsulta);
        }
    }
}
