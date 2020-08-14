using System;
using System.Collections.Generic;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IHistoricoConsultaMatoGrossoSulAppService : IDisposable
    {
        IEnumerable<HistoricoConsultaMatoGrossoSulDTO> BuscarTodos(string filtro);
        void Inserir(HistoricoConsultaMatoGrossoSulDTO historicoConsulta);
        HistoricoConsultaMatoGrossoSulDTO ObterUltimaConsulta();
    }
}
