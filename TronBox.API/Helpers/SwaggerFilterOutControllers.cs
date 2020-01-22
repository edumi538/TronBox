using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace TronBox.API.Helpers
{
    internal class SwaggerFilterOutControllers : IDocumentFilter
    {
        void IDocumentFilter.Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var item in swaggerDoc.Paths.ToList())
            {
                if (!item.Key.ToLower().Contains("/api/v1"))
                    swaggerDoc.Paths.Remove(item.Key);
            }

            swaggerDoc.Extensions.Remove("ConfiguracaoNotificacoesViewModel");
            swaggerDoc.Extensions.Remove("EmpresaViewModel");
            swaggerDoc.Extensions.Remove("InquilinoMongoDTO");
            swaggerDoc.Extensions.Remove("PessoaViewModel");
            swaggerDoc.Extensions.Remove("PessoaDispositivoViewModel");
            swaggerDoc.Extensions.Remove("Pessoa");
            swaggerDoc.Extensions.Remove("Dispositivo");
            swaggerDoc.Extensions.Remove("Cidade");
            swaggerDoc.Extensions.Remove("ValidationResult");
            swaggerDoc.Extensions.Remove("ValidationFailure");
            swaggerDoc.Extensions.Remove("DispositivoViewModel");
            swaggerDoc.Extensions.Remove("PessoaUsuarioViewModel");
            swaggerDoc.Extensions.Remove("PushModel");
            swaggerDoc.Extensions.Remove("IFormFile");
            swaggerDoc.Extensions.Remove("PessoaDTO");
        }
    }
}
