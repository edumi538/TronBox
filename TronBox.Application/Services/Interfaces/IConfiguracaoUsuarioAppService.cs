using System;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IConfiguracaoUsuarioAppService : IDisposable
    {
        ConfiguracaoUsuarioDTO BuscarConfiguracaoUsuario();
        void InserirOuAtualizar(ConfiguracaoUsuarioDTO configuracaoUsuarioDTO);
    }
}
