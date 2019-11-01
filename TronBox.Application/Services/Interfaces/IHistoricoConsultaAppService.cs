using System;
using System.Collections.Generic;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IHistoricoConsultaAppService : IDisposable
    {
        IEnumerable<HistoricoConsultaDTO> BuscarTodos(string filtro);
        void Inserir(HistoricoConsultaDTO historicoConsulta);
        HistoricoConsultaDTO ObterUltimaConsulta();
    }
}
