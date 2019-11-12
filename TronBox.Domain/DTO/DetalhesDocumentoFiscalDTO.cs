using CTe.Classes;
using NFe.Classes;
using TronBox.Domain.Classes.NFSe;
using TronBox.Domain.DTO.InnerClassDTO;

namespace TronBox.Domain.DTO
{
    public class DetalhesDocumentoFiscalDTO
    {
        public int DataArmazenamento { get; set; }
        public bool Cancelado { get; set; }
        public bool Rejeitado { get; set; }
        public bool Denegada { get; set; }
        public DadosOrigemDocumentoFiscalDTO DadosOrigem { get; set; }
        public DadosImportacaoDTO DadosImportacao { get; set; }
        public nfeProc NotaFiscalEletronica { get; set; }
        public cteProc ConhecimentoTransporteEletronico { get; set; }
        public CompNfse NotaFiscalServicoEletronico { get; set; }
    }
}
