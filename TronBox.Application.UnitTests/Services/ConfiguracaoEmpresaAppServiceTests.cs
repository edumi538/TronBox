using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Comum.Domain.Interfaces;
using Moq;
using NUnit.Framework;
using TronBox.Application.Services;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg.Repository;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;

namespace TronBox.Application.UnitTests.Services
{
                
    public class ConfiguracaoEmpresaAppServiceTests
    {
        private ConfiguracaoEmpresaAppService _configuracaoEmpresaAppService;
        private Mock<IRepositoryFactory> _repositoryFactory;
        private Mock<IConfiguracaoEmpresaRepository> _repository;
        private Mock<IBus> _bus;
        private Mock<IMapper> _mapper;
        private Mock<IPessoaUsuarioLogado> _pessoaUsuarioLogado;
        private List<Guid> _tenantIds;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IConfiguracaoEmpresaRepository>();
            _repositoryFactory = new Mock<IRepositoryFactory>();
            _repositoryFactory.Setup(r => r.Instancie<IConfiguracaoEmpresaRepository>())
                .Returns(_repository.Object);

            _bus = new Mock<IBus>();
            _bus.Setup(b => b.RaiseEvent(It.IsAny<DomainNotification>())).Verifiable();
            
            _mapper = new Mock<IMapper>();
            _pessoaUsuarioLogado = new Mock<IPessoaUsuarioLogado>();
            _configuracaoEmpresaAppService = new ConfiguracaoEmpresaAppService(_bus.Object, _mapper.Object,
                _repositoryFactory.Object, _pessoaUsuarioLogado.Object);

            _tenantIds = new List<Guid>();
        }

        [Test]
        public async Task AtualizarTodasCredenciaisPortalEstadual_QuandoListaDeTenantsNull_EmitirEventoNotificacao()
        {
            await _configuracaoEmpresaAppService.AtualizarTodasCredenciaisPortalEstadual(null);
            
            _bus.Verify(b => b.RaiseEvent(It.IsAny<DomainNotification>()), Times.Once);
            _repository.Verify(r => r.BuscarTodos(), Times.Never);
        } 
        
        [Test]
        public async Task AtualizarTodasCredenciaisPortalEstadual_QuandoListaDeTenantsVazio_EmitirEventoNotificacao()
        {
            await _configuracaoEmpresaAppService.AtualizarTodasCredenciaisPortalEstadual(new List<Guid>());
            
            _bus.Verify(b => b.RaiseEvent(It.IsAny<DomainNotification>()), Times.Once);
            _repository.Verify(r => r.BuscarTodos(), Times.Never);
        }
        
        [Test]
        public async Task AtualizarTodasCredenciaisPortalEstadual_QuandoConfiguracaoReferenciaVazia_EmitirEventoNotificacao()
        {
            _tenantIds.Add(new Guid());
            _repository.Setup(r => r.BuscarTodos()).Returns(new List<ConfiguracaoEmpresa>());
            
            await _configuracaoEmpresaAppService.AtualizarTodasCredenciaisPortalEstadual(_tenantIds);
            
            _bus.Verify(b => b.RaiseEvent(It.IsAny<DomainNotification>()), Times.Once);
        }
    }
}