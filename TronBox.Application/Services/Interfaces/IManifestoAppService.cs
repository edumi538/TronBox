using System;
using System.Collections.Generic;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IManifestoAppService : IDisposable
    {
        void Inserir(ManifestoDTO manifestoDTO);
        void Atualizar(Guid id, dynamic manifestoDTO);
        void Deletar(Guid id);
        IEnumerable<ManifestoDTO> BuscarTodos(string filtro);
        ManifestoDTO BuscarPorId(Guid id);
    }
}
