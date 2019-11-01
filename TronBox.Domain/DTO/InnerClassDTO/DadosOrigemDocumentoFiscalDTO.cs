using TronBox.Domain.Enums;
using TronCore.Enumeradores.Helpers;

namespace TronBox.Domain.DTO.InnerClassDTO
{
    public class DadosOrigemDocumentoFiscalDTO
    {
        public EOrigemDocumentoFiscal Origem { get; set; }
        public string Originador { get; set; }

        public string OrigemDescricao
        {
            get => EnumHelper<EOrigemDocumentoFiscal>.GetDisplayValue(Origem);
        }
    }
}
