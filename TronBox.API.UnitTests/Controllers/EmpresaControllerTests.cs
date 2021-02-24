using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TronBox.Application.Services.Interfaces;
using TronBox.UI.Controllers;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;

namespace TronBox.API.UnitTests.Controllers
{
    public class EmpresaControllerTests
    {
        private readonly List<Guid> _tenantsIds = new List<Guid> {new Guid()};
        private Mock<IAppServiceFactory> _appServiceFactory;
        private Mock<IConfiguracaoEmpresaAppService> _service;
        private EmpresaController _empresaController;
        private Mock<IDomainNotificationHandler<DomainNotification>> _notifications;

        [SetUp]
        public void SetUp()
        {
            _service = new Mock<IConfiguracaoEmpresaAppService>();
            _service.Setup(s => s.AtualizarTodasCredenciaisPortalEstadual(It.IsAny<List<Guid>>()));
            _appServiceFactory = new Mock<IAppServiceFactory>();
            _appServiceFactory.Setup(s => s.Instancie<IConfiguracaoEmpresaAppService>())
                .Returns(_service.Object);

            _notifications = new Mock<IDomainNotificationHandler<DomainNotification>>();
            _empresaController = new EmpresaController(_notifications.Object, _appServiceFactory.Object);
        }

        [Test]
        public async Task ReplicarConfiguracoesAcesso_NaoExisteNotificacao_RetornaOk()
        {
            _notifications.Setup(n => n.HasNotifications())
                .Returns(false);

            var result = await _empresaController.ReplicarConfiguracoesAcesso(_tenantsIds);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task ReplicarConfiguracoesAcesso_ExisteNotificacao_RetornaBadRequest()
        {
            _notifications.Setup(n => n.HasNotifications())
                .Returns(true);

            _notifications.Setup(n => n.GetNotifications())
                .Returns(new List<DomainNotification>
                {
                    new DomainNotification("a", "b")
                });

            var result = await _empresaController.ReplicarConfiguracoesAcesso(_tenantsIds);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}