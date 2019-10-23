using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos;
using System;

namespace TronBox.Application.Services.Interfaces
{
    public interface IEnumeradorAppService : IDisposable
    {
        string ObterCSTICMS(Csticms csticms);
    }
}
