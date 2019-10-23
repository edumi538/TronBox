using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos;
using NFe.Utils;
using TronBox.Application.Services.Interfaces;

namespace TronBox.Application.Services
{
    public class EnumeradorAppService : IEnumeradorAppService
    {
        public void Dispose()
        {
        }

        public string ObterCSTICMS(Csticms csticms) => Conversao.CsticmsParaString(csticms);
    }
}
