using System.Collections.Generic;

namespace TronBox.Domain.DTO
{
    public class RetornoUploadDTO
    {
        public IEnumerable<RetornoDocumentoFiscalDTO> DocumentosInseridos { get; set; }
        public IEnumerable<RetornoDocumentoFiscalNaoInseridoDTO> DocumentosNaoInseridos { get; set; }

        public RetornoUploadDTO(IEnumerable<RetornoDocumentoFiscalDTO> documentosInseridos, IEnumerable<RetornoDocumentoFiscalNaoInseridoDTO> documentosNaoInseridos)
        {
            DocumentosInseridos = documentosInseridos;
            DocumentosNaoInseridos = documentosNaoInseridos;
        }
    }

    public class RetornoDocumentoFiscalNaoInseridoDTO
    {
        public string NomeArquivo { get; set; }
        public string Mensagem { get; set; }
        public dynamic Erros { get; set; }

        public RetornoDocumentoFiscalNaoInseridoDTO(string nomeArquivo, string mensagem, dynamic erros)
        {
            NomeArquivo = nomeArquivo;
            Mensagem = mensagem;
            Erros = erros;
        }
    }
}
