using Microsoft.Azure.NotificationHubs;

namespace TGCW.Infra.IoC.Services
{
    public class Notificacoes
    {
        public static Notificacoes Instancia = new Notificacoes();

        public NotificationHubClient Hub { get; set; }

        private Notificacoes()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://tgcwapp.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=ujHgJUxCpjnseRYrygA6pT/QncK5/dZqvDFShKEsSlA=", "tgcw");
        }
    }
}
