using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IManifestoAppService : IDisposable
    {
        Task<RespostaManifestacaoDTO> Manifestar(ManifestarDTO manifestarDTO);
        int InserirOuAtualizar(IEnumerable<dynamic> manifestosDTO);
        void Deletar(Guid id);
        IEnumerable<ManifestoDTO> BuscarTodos(string filtro);
        IEnumerable<ManifestoDTO> BuscarPorChaves(IEnumerable<string> chaves);
        ManifestoDTO BuscarPorId(Guid id);
    }
}
