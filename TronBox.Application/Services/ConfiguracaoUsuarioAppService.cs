using AutoMapper;
using Comum.Domain.Aggregates.PessoaAgg;
using Comum.Domain.Interfaces;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.ConfiguracaoUsuarioAgg;
using TronBox.Domain.Aggregates.ConfiguracaoUsuarioAgg.Repository;
using TronBox.Domain.DTO;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;

namespace TronBox.Application.Services
{
    public class ConfiguracaoUsuarioAppService : IConfiguracaoUsuarioAppService
    {
        #region Membros
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IPessoaUsuarioLogado _usuarioLogado;
        #endregion

        #region Construtor
        public ConfiguracaoUsuarioAppService(IBus bus, IMapper mapper, IRepositoryFactory repositoryFactory, IPessoaUsuarioLogado usuarioLogado)
        {
            _bus = bus;
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
            _usuarioLogado = usuarioLogado;
        }
        #endregion

        public void Dispose()
        {
        }

        public ConfiguracaoUsuarioDTO BuscarConfiguracaoUsuario()
        {
            var pessoaLogada = ObterPessoaLogada();

            if (pessoaLogada == null) return null;

            var configuracaoUsuario = _repositoryFactory.Instancie<IConfiguracaoUsuarioRepository>().BuscarPorExpressao(c => c.Inscricao == pessoaLogada.Cpf);

            if (configuracaoUsuario == null)
            {
                _bus.RaiseEvent(new DomainNotification("ConfiguracaoNaoEncontrada", "Configurações do Usuário não foram encontradas."));
                return null;
            }

            return _mapper.Map<ConfiguracaoUsuarioDTO>(configuracaoUsuario);
        }

        public void InserirOuAtualizar(ConfiguracaoUsuarioDTO configuracaoUsuarioDTO)
        {
            if (configuracaoUsuarioDTO == null)
            {
                _bus.RaiseEvent(new DomainNotification("NaoEncontrado", "Configuração de Usuário não informado ou está inválido."));
                return;
            }

            var pessoaLogada = ObterPessoaLogada();

            if (pessoaLogada == null) return;

            if (string.IsNullOrEmpty(configuracaoUsuarioDTO.Inscricao)) configuracaoUsuarioDTO.Inscricao = pessoaLogada.Cpf;

            var configuracaoUsuario = _mapper.Map<ConfiguracaoUsuario>(configuracaoUsuarioDTO);

            if (configuracaoUsuario.EhValido())
            {
                var configuracaoUsuarioExistente = _repositoryFactory.Instancie<IConfiguracaoUsuarioRepository>().BuscarPorExpressao(c => c.Inscricao == pessoaLogada.Cpf);

                if (configuracaoUsuarioExistente != null)
                {
                    configuracaoUsuario.Id = configuracaoUsuarioExistente.Id;

                    _repositoryFactory.Instancie<IConfiguracaoUsuarioRepository>().Atualizar(configuracaoUsuario);
                }
                else
                {
                    configuracaoUsuario.Inscricao = pessoaLogada.Cpf;

                    _repositoryFactory.Instancie<IConfiguracaoUsuarioRepository>().Inserir(configuracaoUsuario);
                }
            }

            PublicarErros(configuracaoUsuario);
        }

        public void Inserir(ConfiguracaoUsuarioDTO configuracaoUsuarioDTO)
        {
            if (configuracaoUsuarioDTO == null)
            {
                _bus.RaiseEvent(new DomainNotification("NaoEncontrado", "Configuração de Usuário não informado ou está inválido."));
                return;
            }

            var configuracaoUsuario = _mapper.Map<ConfiguracaoUsuario>(configuracaoUsuarioDTO);

            if (configuracaoUsuario.EhValido()) _repositoryFactory.Instancie<IConfiguracaoUsuarioRepository>().Inserir(configuracaoUsuario);
        }

        #region Private Methods
        private Pessoa ObterPessoaLogada()
        {
            var pessoaUsuarioLogado = _usuarioLogado.ObtenhaPessoa();

            if (pessoaUsuarioLogado == null)
            {
                _bus.RaiseEvent(new DomainNotification("PessoaUsuarioNaoEncontrada", "Usuário da Pessoa não encontrado."));
                return null;
            }

            var pessoaLogada = pessoaUsuarioLogado.Pessoa;

            if (pessoaLogada == null)
            {
                _bus.RaiseEvent(new DomainNotification("PessoaLogadaNaoEncontrada", "Pessoa logada não encontrada."));
                return null;
            }

            return pessoaLogada;
        }

        private void PublicarErros(ConfiguracaoUsuario configuracaoUsuario)
        {
            foreach (var error in configuracaoUsuario.ValidationResult.Errors)
                _bus.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));
        }
        #endregion
    }
}
