using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using TronBox.Domain.Enums;
using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.HistoricoConsultaAgg
{
    public class HistoricoConsulta : Entity<HistoricoConsulta>
    {
        public ETipoDocumentoConsulta TipoDocumentoConsulta { get; set; }
        public ETipoConsulta TipoConsulta { get; set; }
        public long DataHoraConsulta { get; set; }
        [BsonIgnoreIfDefault]
        public string UltimoNSU { get; set; }
        [BsonIgnoreIfDefault]
        public int DocumentosEncontrados { get; set; }
        [BsonIgnoreIfDefault]
        public int DocumentosArmazenados { get; set; }
    }

    public class HistoricoConsultaValidator : AbstractValidator<HistoricoConsulta>
    {
        public HistoricoConsultaValidator()
        {
            RuleFor(a => a.TipoDocumentoConsulta)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Tipo de Documento"));

            RuleFor(a => a.TipoConsulta)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Tipo da Consulta"));

            RuleFor(a => a.DataHoraConsulta)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Data/Hora da Consulta"));
        }
    }
}
