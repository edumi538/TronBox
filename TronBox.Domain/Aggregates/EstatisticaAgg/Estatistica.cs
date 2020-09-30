using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.EstatisticaAgg
{
    public class Estatistica : Entity<Estatistica>
    {
        public long DataHora { get; set; }
        [BsonIgnoreIfDefault]
        public bool CertificadoAtivo { get; set; }
        [BsonIgnoreIfDefault]
        public int NotaFiscalEntrada { get; set; }
        [BsonIgnoreIfDefault]
        public int NotaFiscalSaida { get; set; }
        [BsonIgnoreIfDefault]
        public int NotaFiscalConsumidor { get; set; }
        [BsonIgnoreIfDefault]
        public int NotaFiscalServicoEntrada { get; set; }
        [BsonIgnoreIfDefault]
        public int NotaFiscalServicoSaida { get; set; }
        [BsonIgnoreIfDefault]
        public int ConhecimentoTransporteEntrada { get; set; }
        [BsonIgnoreIfDefault]
        public int ConhecimentoTransporteSaida { get; set; }
        [BsonIgnoreIfDefault]
        public int ConhecimentoTransporteNaoTomador { get; set; }
        [BsonIgnoreIfDefault]
        public int OrigemEmail { get; set; }
        [BsonIgnoreIfDefault]
        public int OrigemUploadManual { get; set; }
        [BsonIgnoreIfDefault]
        public int OrigemDownloadAgente { get; set; }
        [BsonIgnoreIfDefault]
        public int OrigemAgenteManifestacao { get; set; }
        [BsonIgnoreIfDefault]
        public int OrigemMonitorA3 { get; set; }
        [BsonIgnoreIfDefault]
        public int OrigemPortalEstadual { get; set; }
        [BsonIgnoreIfDefault]
        public int OrigemMonitorSincronizacao { get; set; }

        public bool EhValido()
        {
            ValidationResult = new EstatisticaValidator().Validate(this);

            return ValidationResult.IsValid;
        }
    }

    public class EstatisticaValidator : AbstractValidator<Estatistica>
    {
        public EstatisticaValidator()
        {
            RuleFor(c => c.DataHora)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Data e Hora"));
        }
    }
}