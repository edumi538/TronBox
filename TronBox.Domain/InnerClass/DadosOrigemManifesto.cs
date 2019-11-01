using FluentValidation;
using TronBox.Domain.Enums;
using TronCore.Dominio.Base;

namespace TronBox.Domain.InnerClass
{
    public class DadosOrigemManifesto
    {
        public EOrigemManifesto Origem { get; set; }
        public string Originador { get; set; }
    }

    public class DadosOrigemManifestoValidator : AbstractValidator<DadosOrigemManifesto>
    {
        public DadosOrigemManifestoValidator()
        {
            RuleFor(a => a.Origem)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Origem"));

            RuleFor(a => a.Originador)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Originador"));
        }
    }
}
