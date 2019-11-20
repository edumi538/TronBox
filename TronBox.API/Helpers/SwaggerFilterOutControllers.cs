using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace TronBox.API.Helpers
{
    internal class SwaggerFilterOutControllers : IDocumentFilter
    {

        void IDocumentFilter.Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var item in swaggerDoc.Paths.ToList())
            {
                if (!item.Key.ToLower().Contains("/api/v1"))
                    swaggerDoc.Paths.Remove(item.Key);
            }

            swaggerDoc.Definitions.Remove("ConfiguracaoNotificacoesViewModel");
            swaggerDoc.Definitions.Remove("EmpresaViewModel");
            swaggerDoc.Definitions.Remove("InquilinoMongoDTO");
            swaggerDoc.Definitions.Remove("PessoaViewModel");
            swaggerDoc.Definitions.Remove("PessoaDispositivoViewModel");
            swaggerDoc.Definitions.Remove("Pessoa");
            swaggerDoc.Definitions.Remove("Dispositivo");
            swaggerDoc.Definitions.Remove("Cidade");
            swaggerDoc.Definitions.Remove("ValidationResult");
            swaggerDoc.Definitions.Remove("ValidationFailure");
            swaggerDoc.Definitions.Remove("DispositivoViewModel");
            swaggerDoc.Definitions.Remove("PessoaUsuarioViewModel");
            swaggerDoc.Definitions.Remove("PushModel");
            swaggerDoc.Definitions.Remove("IFormFile");
            swaggerDoc.Definitions.Remove("PessoaDTO");
        }
    }
}
