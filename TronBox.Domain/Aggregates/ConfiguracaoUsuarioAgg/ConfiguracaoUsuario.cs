using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.ConfiguracaoUsuarioAgg
{
    public class ConfiguracaoUsuario : Entity<ConfiguracaoUsuario>
    {
        public string Inscricao { get; set; }
        [BsonIgnoreIfDefault]
        public bool NotificarPortalEstadual { get; set; }

        public bool EhValido()
        {
            ValidationResult = new ConfiguracaoUsuarioValidator().Validate(this);

            return ValidationResult.IsValid;
        }
    }

    public class ConfiguracaoUsuarioValidator : AbstractValidator<ConfiguracaoUsuario>
    {
        public ConfiguracaoUsuarioValidator()
        {
            RuleFor(a => a.Inscricao)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Inscrição"));
        }
    }
}
