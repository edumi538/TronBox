﻿using TronBox.Domain.DTO.InnerClassDTO;
using TronBox.Domain.Enums;

namespace TronBox.Domain.DTO
{
    public class DocumentoFiscalDTO
    {
        public string Id { get; set; }
        public TipoDocumentoFiscal TipoDocumentoFiscal { get; set; }
        public string ChaveDocumentoFiscal { get; set; }
        public string NumeroDocumentoFiscal { get; set; }
        public double ValorDocumentoFiscal { get; set; }
        public string SerieDocumentoFiscal { get; set; }
        public int DataArmazenamento { get; set; }
        public int DataEmissaoDocumento { get; set; }
        public bool Cancelado { get; set; }
        public bool Rejeitado { get; set; }
        public bool Denegada { get; set; }
        public DadosOrigemDocumentoFiscalDTO DadosOrigem { get; set; }
        public DadosImportacaoDTO DadosImportacao { get; set; }
        public DadosFornecedorDTO DadosEmitenteDestinatario { get; set; }
        public string CaminhoArquivo { get; set; }
    }
}
