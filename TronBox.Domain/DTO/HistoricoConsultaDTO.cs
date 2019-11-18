using System;
using TronBox.Domain.Enums;
using TronCore.Enumeradores.Helpers;
using TronCore.Utilitarios;

namespace TronBox.Domain.DTO
{
    public class HistoricoConsultaDTO
    {
        public string Id { get; set; }
        public ETipoConsulta TipoConsulta { get; set; }
        public long DataHoraConsulta { get; set; }
        public int DocumentosEncontrados { get; set; }
        public int DocumentosArmazenados { get; set; }
        public string DescricaoTipo
        {
            get => EnumHelper<ETipoConsulta>.GetDisplayValue(TipoConsulta);
        }
        public DateTime DataEmissaoDocumentoFormatada
        {
            get => UtilitarioDatas.ConvertIntToDateTime(DataHoraConsulta);
        }
    }
}
