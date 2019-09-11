﻿using Comum.Domain.Services.Interfaces;
using Comum.Domain.ViewModels;
using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TronBox.Domain.Enums;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;
using TronCore.Seguranca.AOP.Anotacao;

namespace TronBox.UI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/empresas")]
    [IdentificadorFuncao(typeof(eFuncaoTronBox), eFuncaoTronBox.ID_EMPRESA)]
    public class EmpresaController : BaseController
    {
        readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public EmpresaController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
        }

        [HttpGet]
        [IdentificadorOperacao(eFuncaoTronBox.ID_EMPRESA, "Carregar Empresa", eOperacaoSuite.ID_OP_ACESSO, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/empresas")]
        public IActionResult Get() => Ok(AppServiceFactory.Instancie<IEmpresaAppService>().BuscarTodos().FirstOrDefault());

        [HttpPut]
        [IdentificadorOperacao(eFuncaoTronBox.ID_EMPRESA, "Atualizar Empresa", eOperacaoSuite.ID_OP_EDITAR, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/empresas/editar/:id")]
        public IActionResult Put([FromBody]EmpresaViewModel empresa)
        {
            AppServiceFactory.Instancie<IEmpresaAppService>().Atualizar(empresa);

            if (_notifications.HasNotifications())
            {
                return BadRequest(new
                {
                    sucesso = false,
                    erro = _notifications.GetNotifications()
                        .Select(c => new
                        {
                            Chave = c.Key,
                            Mensagem = c.Value
                        })
                });
            }

            return Ok(new
            {
                sucesso = true,
                mensagem = "Operação realizada com sucesso."
            });
        }
    }
}