using System;
using System.Collections.Generic;
using TronBox.Domain.Enums;
using TronCore.Enumeradores.Helpers;
using TronCore.Utilitarios;

namespace TronBox.Domain.DTO
{
    public class HistoricoConsultaMatoGrossoDTO
    {
        public string Id { get; set; }
        public string InscricaoEstadual { get; set; }
        public ETipoConsulta TipoConsulta { get; set; }
        public long DataHoraConsulta { get; set; }
        public int DataInicialConsultada { get; set; }
        public int DataFinalConsultada { get; set; }
        public IEnumerable<string> ChavesEncontradas { get; set; }
        public string DescricaoTipoConsulta
        {
            get => EnumHelper<ETipoConsulta>.GetDisplayValue(TipoConsulta);

        }
        public DateTime DataHoraConsultaFormatada
        {
            get => UtilitarioDatas.ConvertIntToDateTime(DataHoraConsulta);
        }
        public DateTime DataInicialConsultadaFormatada
        {
            get => UtilitarioDatas.ConvertIntToDate(DataInicialConsultada);
        }
        public DateTime DataFinalConsultadaFormatada
        {
            get => UtilitarioDatas.ConvertIntToDate(DataFinalConsultada);
        }
    }
}
