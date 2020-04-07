using System;
using TronBox.Domain.DTO.InnerClassDTO;
using TronBox.Domain.Enums;
using TronCore.Enumeradores.Helpers;
using TronCore.Utilitarios;

namespace TronBox.Domain.DTO
{
    public class DocumentoFiscalDTO
    {
        public string Id { get; set; }
        public string InscricaoEstadual { get; set; }
        public ETipoDocumentoFiscal TipoDocumentoFiscal { get; set; }
        public string ChaveDocumentoFiscal { get; set; }
        public string NumeroDocumentoFiscal { get; set; }
        public double ValorDocumentoFiscal { get; set; }
        public string SerieDocumentoFiscal { get; set; }
        public string NsuDocumentoFiscal { get; set; }
        public long DataArmazenamento { get; set; }
        public int DataEmissaoDocumento { get; set; }
        public bool Cancelado { get; set; }
        public bool Rejeitado { get; set; }
        public bool Denegada { get; set; }
        public DadosOrigemDocumentoFiscalDTO DadosOrigem { get; set; }
        public DadosImportacaoDTO DadosImportacao { get; set; }
        public DadosFornecedorDTO DadosEmitenteDestinatario { get; set; }
        public string CaminhoArquivo { get; set; }
        public string NomeArquivo { get; set; }
        public DateTime DataArmazenamentoFormatada
        {
            get => UtilitarioDatas.ConvertIntToDateTime(DataArmazenamento);
        }
        public DateTime DataEmissaoDocumentoFormatada
        {
            get => UtilitarioDatas.ConvertIntToDate(DataEmissaoDocumento);
        }
        public string DescricaoTipo
        {
            get => EnumHelper<ETipoDocumentoFiscal>.GetDisplayValue(TipoDocumentoFiscal);
        }
        public bool Processado { get => DadosImportacao != null && DadosImportacao.DataImportacao > 0; }

        public DocumentoFiscalDTO()
        {
        }

        public DocumentoFiscalDTO(bool cancelado, string chaveDocumentoFiscal)
        {
            Cancelado = cancelado;
            ChaveDocumentoFiscal = chaveDocumentoFiscal;
        }
    }
}
