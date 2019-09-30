using System;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IConfiguracaoEmpresaAppService : IDisposable
    {
        EmpresaDTO BuscarEmpresa();
        void AtualizarEmpresa(EmpresaDTO empresaDto);
    }
}
