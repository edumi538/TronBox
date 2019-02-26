using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.CustomerAgg
{
    public class Customer: Entity <Customer>
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }
}
