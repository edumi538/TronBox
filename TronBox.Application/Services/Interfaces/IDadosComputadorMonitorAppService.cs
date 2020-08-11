using System;
using System.Collections.Generic;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IDadosComputadorMonitorAppService : IDisposable
    {
        IEnumerable<DadosComputadorMonitorDTO> BuscarTodos(string filtro);
        void Inserir(DadosComputadorMonitorDTO dadosComputadorMonitorDTO);
    }
}
