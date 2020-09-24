using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IEstatisticaAppService : IDisposable
    {
        IEnumerable<EstatisticaDTO> BuscarTodos(string filtro);
        Task<EstatisticaDTO> Calcular(long dataHora);
    }
}
