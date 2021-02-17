using Comum.Domain.ViewModels;
using Comum.DTO;
using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;
using TronCore.Seguranca.AOP.Anotacao;

namespace TronBox.UI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/empresas")]
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
        public IActionResult Get() => Ok(AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().BuscarEmpresa());

        [HttpPut]
        [IdentificadorOperacao(eFuncaoTronBox.ID_EMPRESA, "Atualizar Empresa", eOperacaoSuite.ID_OP_EDITAR, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/empresas/editar/:id")]
        public IActionResult Put([FromBody] EmpresaDTO empresa)
        {
            AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().AtualizarEmpresa(empresa);

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

        [HttpPost("pessoa")]
        public IActionResult Post([FromBody] PessoaUsuarioDTO pessoaUsuario)
        {
            AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().CriarPessoa(pessoaUsuario);

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

        [HttpPut("pessoa")]
        public IActionResult Put([FromBody] PessoaViewModel pessoa)
        {
            AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().AlterarPessoa(pessoa);

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

        [HttpPost("certificado/upload")]
        public async Task<IActionResult> Upload([FromForm] CertificadoCreateDTO certificadoCreateDTO)
        {
            var resposta = await AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().Upload(certificadoCreateDTO);

            if (!resposta.Sucesso)
                return BadRequest(resposta);

            return Ok(resposta);
        }

        [HttpDelete("certificado/{id:GUID}")]
        public async Task<IActionResult> DeleteCertificado(Guid id)
        {
            var resposta = await AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().DeletarCertificado(id);

            if (!resposta.Sucesso)
                return BadRequest(resposta);

            return Ok(resposta);
        }

        [HttpPut("atualizar-email")]
        public IActionResult AtualizarEmail([FromBody] AtualizacaoEmailDTO empregado)
        {
            AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().AtualizarEmail(empregado);

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

        [HttpGet("situacao-certificado")]
        public IActionResult SituacaoCertificado() => Ok(AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().SituacaoCertificado());

        [HttpPost("notificar-acesso-invalido")]
        public async Task<IActionResult> NotificarContadorAcessoInvalido([FromBody] NotificaSenhaInvalidaDTO sefaz)
        {
            await AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().NotificarContadorAcessoInvalido(sefaz.Estado);

            return Ok(new
            {
                sucesso = true,
                mensagem = "Operação realizada com sucesso."
            });
        }
        
        [HttpPut("replicar-configuracao-acesso")]
        public async Task<IActionResult> ReplicarConfiguracoesAcesso([FromBody] IEnumerable<Guid> tenantIds)
        {
            await AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().AtualizarTodasCredenciaisPortalEstadual(tenantIds);
            
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