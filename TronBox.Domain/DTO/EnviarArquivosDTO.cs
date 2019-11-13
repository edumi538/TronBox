using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using TronBox.Domain.Enums;

namespace TronBox.Domain.DTO
{
    public class EnviarArquivosDTO
    {
        public EOrigemDocumentoFiscal Origem { get; set; }
        public string Originador { get; set; }
        public List<IFormFile> Arquivos { get; set; }
        public List<DetalhesEnvioDTO> DetalhesEnvio { get; set; }
        public class DetalhesEnvioDTO
        {
            public string ChaveDocumentoFiscal { get; set; }
            public string NsuDocumentoFiscal { get; set; }
        }
    }
}
