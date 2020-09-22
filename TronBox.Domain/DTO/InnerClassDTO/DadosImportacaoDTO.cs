using System;
using TronCore.Utilitarios;

namespace TronBox.Domain.DTO.InnerClassDTO
{
    public class DadosImportacaoDTO
    {
        public long DataImportacao { get; set; }
        public DateTime DataImportacaoFormatada { get => UtilitarioDatas.ConvertIntToDateTime(DataImportacao); }
        public string Usuario { get; set; }
        public bool Desfazer { get; set; }
        public string ChaveDocumentoFiscal{ get; set; }
    }
}
