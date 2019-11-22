using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IManifestoAppService : IDisposable
    {
        Task<RespostaManifestacaoDTO> Manifestar(ManifestarDTO manifestarDTO);
        void Inserir(ManifestoDTO manifestoDTO);
        void Atualizar(Guid id, dynamic manifestoDTO);
        void Deletar(Guid id);
        IEnumerable<ManifestoDTO> BuscarTodos(string filtro);
        ManifestoDTO BuscarPorId(Guid id);
    }
}
