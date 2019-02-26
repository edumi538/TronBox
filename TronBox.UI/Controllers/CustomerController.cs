using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.DTO;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;

namespace TronBox.UI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/customers")]
    public class CustomerController : BaseController
    {
        IDomainNotificationHandler<DomainNotification> _notifications;

        public CustomerController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
        }

        [HttpGet]
        public IActionResult Get(string filtro)
        {
            try
            {
                IEnumerable<CustomerDTO> customer = AppServiceFactory.Instancie<ICustomerAppService>().BuscarTodos(filtro);

                var settings = new JsonSerializerSettings();

                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;

                return Json(customer, settings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, erro = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]CustomerDTO customer)
        {
            try
            {
                if (customer.Id == null)
                    customer.Id = Guid.NewGuid().ToString();

                AppServiceFactory.Instancie<ICustomerAppService>().Inserir(customer);

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

                return Created(customer.Id,
                    new
                    {
                        sucesso = true,
                        mensagem = "Operação realizada com sucesso."
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, erro = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody]CustomerDTO customer)
        {
            try
            {
                AppServiceFactory.Instancie<ICustomerAppService>().Atualizar(customer);

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
            catch (Exception ex)
            {

                return BadRequest(new { sucesso = false, erro = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                AppServiceFactory.Instancie<ICustomerAppService>().Deletar(id);

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
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, erro = ex.Message });
            }
        }


    }
}