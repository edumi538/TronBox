using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.DadosComputadorMonitorAgg
{
    public class DadosComputadorMonitor : Entity<DadosComputadorMonitor>
    {
        public long DataHora { get; set; }
        public string Usuario { get; set; }
        [BsonIgnoreIfNull]
        public string SistemaOperacional { get; set; }
        [BsonIgnoreIfNull]
        public string VersaoSistemaOperacional { get; set; }
        [BsonIgnoreIfNull]
        public int ArquiteturaSistemaOperacional { get; set; }
        [BsonIgnoreIfNull]
        public string NomeComputador { get; set; }
        [BsonIgnoreIfNull]
        public string NomeUsuario { get; set; }
        [BsonIgnoreIfNull]
        public string Processador { get; set; }
        [BsonIgnoreIfNull]
        public int ArquiteturaProcessador { get; set; }
        [BsonIgnoreIfDefault]
        public long MemoriaRam { get; set; }
    }

    public class DadosComputadorMonitorValidator : AbstractValidator<DadosComputadorMonitor>
    {
        public DadosComputadorMonitorValidator()
        {
            RuleFor(a => a.DataHora)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Data e Hora"));

            RuleFor(a => a.Usuario)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Usuário"));
        }
    }
}
