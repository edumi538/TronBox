﻿using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Enums;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;
using TronCore.Seguranca.AOP.Anotacao;

namespace TronBox.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/dashboards")]
    [IdentificadorFuncao(typeof(eFuncaoTronBox), eFuncaoTronBox.ID_DASHBOARD)]
    public class DashboardController : BaseController
    {
        public DashboardController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
        }

        [HttpGet("total-sem-manifesto")]
        [IdentificadorOperacao(eFuncaoTronBox.ID_DASHBOARD, "Obter Dados do Dashboard", eOperacaoSuite.ID_OP_ACESSO, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/dashboards")]
        public IActionResult ContarSemManifesto() => Ok(AppServiceFactory.Instancie<IDashboardAppService>().ContarSemManifesto());

        [HttpGet("documento-fiscal")]
        public IActionResult GetDocumentosArmazenados(int dataInicial, int dataFinal) => Ok(AppServiceFactory.Instancie<IDashboardAppService>().ObterDadosDocumentosArmazenados(dataInicial, dataFinal));

        [HttpGet("origem")]
        public IActionResult GetOrigemDocumentos(int dataInicial, int dataFinal) => Ok(AppServiceFactory.Instancie<IDashboardAppService>().ObterDadosOrigemDocumentos(dataInicial, dataFinal));

        [HttpGet("ultima-semana")]
        public IActionResult GetDadosUltimaSemana(int dataInicial, int dataFinal) => Ok(AppServiceFactory.Instancie<IDashboardAppService>().ObterDadosUltimaSemana(dataInicial, dataFinal));
    }
}