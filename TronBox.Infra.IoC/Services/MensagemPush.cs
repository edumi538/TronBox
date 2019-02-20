using Microsoft.Azure.NotificationHubs;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGCW.Domain.Enums;
using TGCW.Domain.Interfaces;

namespace TGCW.Infra.IoC.Services
{
    public class MensagemPush : IMensagemPush
    {
        private NotificationHubClient _hub;

        public MensagemPush()
        {
            _hub = Notificacoes.Instancia.Hub;
        }

        public async Task NotificarUsuarioAsync(List<string> emails, string titulo, string mensagem, ePlataforma plataforma = ePlataforma.Todos)
        {
            string notificacaoIos = "{ \"aps\": { \"alert\": { \"title\": \"" + titulo + "\", \"body\": \"" + mensagem + "\" }}}";
            string notificacaoAndroid = "{ \"data\" : {\"id\": \"123\", \"title\":\"" + titulo + "\",  \"message\":\"" + mensagem + "\"}}";

            if (plataforma == ePlataforma.Todos)
            {
                await _hub.SendGcmNativeNotificationAsync(notificacaoAndroid, emails);
                await _hub.SendAppleNativeNotificationAsync(notificacaoIos, emails);

            }
            else if (plataforma == ePlataforma.Android)
            {
                await _hub.SendGcmNativeNotificationAsync(notificacaoAndroid, emails);
            }
            else if (plataforma == ePlataforma.IOS)
            {
                await _hub.SendAppleNativeNotificationAsync(notificacaoIos, emails);
            }
        }

        public async Task NotificarUsuarioAsync(string email, string titulo, string mensagem, ePlataforma plataforma = ePlataforma.Todos)
        {
            string notificacaoIos = "{\"aps\": {\"alert\":\"" + mensagem + "\"}}";
            string notificacaoAndroid = "{ \"data\" : {\"id\": \"123\", \"title\":\"" + titulo + "\",  \"message\":\"" + mensagem + "\"}}";

            if (plataforma == ePlataforma.Todos)
            {
                await _hub.SendGcmNativeNotificationAsync(notificacaoAndroid, email);
                await _hub.SendAppleNativeNotificationAsync(notificacaoIos, email);
            }
            else if (plataforma == ePlataforma.Android)
            {
                await _hub.SendGcmNativeNotificationAsync(notificacaoAndroid, email);
            }
            else if (plataforma == ePlataforma.IOS)
            {
                await _hub.SendAppleNativeNotificationAsync(notificacaoIos, email);
            }
        }
    }
}
