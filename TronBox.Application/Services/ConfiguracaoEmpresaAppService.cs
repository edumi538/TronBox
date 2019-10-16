using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using Comum.Domain.Services.Interfaces;
using Comum.Domain.ViewModels;
using FluentValidation.Results;
using System.Linq;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg.Repository;
using TronBox.Domain.DTO;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.InjecaoDependencia;
using TronCore.Persistencia.Interfaces;

namespace TronBox.Application.Services
{
    public class ConfiguracaoEmpresaAppService : IConfiguracaoEmpresaAppService
    {
        #region Membros
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;
        #endregion

        #region Construtor
        public ConfiguracaoEmpresaAppService(IBus bus, IMapper mapper, IRepositoryFactory repositoryFactory)
        {
            _bus = bus;
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
        }
        #endregion

        public void Dispose()
        {
        }

        public EmpresaDTO BuscarEmpresa()
        {
            var empresa = _mapper.Map<EmpresaDTO>(_repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault());

            empresa.ConfiguracaoEmpresa = _mapper.Map<ConfiguracaoEmpresaDTO>(BuscarConfiguracaoEmpresa());

            return empresa;
        }

        public void AtualizarEmpresa(EmpresaDTO empresaDto)
        {
            var empresa = _mapper.Map<Empresa>(empresaDto);
            var configuracaoEmpresa = _mapper.Map<ConfiguracaoEmpresa>(empresaDto.ConfiguracaoEmpresa);
            configuracaoEmpresa.Inscricao = empresa.Inscricao;

            if (EhValido(configuracaoEmpresa))
            {
                var configuracaoExistente = BuscarConfiguracaoEmpresa();

                if (configuracaoExistente != null)
                {
                    configuracaoEmpresa.Id = configuracaoExistente.Id;
                    _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().Atualizar(configuracaoEmpresa);
                }
                else _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().Inserir(configuracaoEmpresa);

                FabricaGeral.Instancie<IEmpresaAppService>().Atualizar(_mapper.Map<EmpresaViewModel>(empresa));
            }
        }

        #region Private Methods
        private ConfiguracaoEmpresa BuscarConfiguracaoEmpresa() => _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().BuscarTodos().FirstOrDefault();

        private bool EhValido(ConfiguracaoEmpresa configuracaoEmpresa)
        {
            var validator = (new ConfiguracaoEmpresaValidator()).Validate(configuracaoEmpresa);

            CriarMensagensErro(validator);

            return validator.IsValid;
        }

        private void CriarMensagensErro(ValidationResult validator)
        {
            foreach (var error in validator.Errors)
                _bus.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));
        }
        #endregion
    }
}
