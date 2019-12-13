using Comum.DTO;
using System;
using System.Threading.Tasks;
using TronBox.Domain.DTO;
using TronCore.Dominio.Base;

namespace TronBox.Application.Services.Interfaces
{
    public interface IConfiguracaoEmpresaAppService : IDisposable
    {
        Task<Resposta> Upload(CertificadoCreateDTO certificadoCreateDTO);
        Task<Resposta> DeletarCertificado(Guid id);
        EmpresaDTO BuscarEmpresa();
        void AtualizarEmpresa(EmpresaDTO empresaDto);
        void AtualizarEmail(AtualizacaoEmailDTO empregado);
        CertificadoSituacaoDTO SituacaoCertificado();
    }
}
