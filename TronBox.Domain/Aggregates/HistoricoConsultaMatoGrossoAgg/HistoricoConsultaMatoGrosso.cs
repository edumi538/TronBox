using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using TronBox.Domain.Enums;
using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoAgg
{
    public class HistoricoConsultaMatoGrosso : Entity<HistoricoConsultaMatoGrosso>
    {
        public ETipoConsulta TipoConsulta { get; set; }
        public long DataHoraConsulta { get; set; }
        [BsonIgnoreIfDefault]
        public int DataInicialConsultada { get; set; }
        [BsonIgnoreIfDefault]
        public int DataFinalConsultada { get; set; }
        [BsonIgnoreIfDefault]
        public IEnumerable<string> ChavesEncontradas { get; set; }
    }

    public class HistoricoConsultaMatoGrossoValidator : AbstractValidator<HistoricoConsultaMatoGrosso>
    {
        public HistoricoConsultaMatoGrossoValidator()
        {
            RuleFor(a => a.TipoConsulta)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Tipo da Consulta"));

            RuleFor(a => a.DataHoraConsulta)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Data/Hora da Consulta"));
        }
    }
}
