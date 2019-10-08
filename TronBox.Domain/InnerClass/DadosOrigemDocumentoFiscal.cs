using FluentValidation;
using TronBox.Domain.Enums;
using TronCore.Dominio.Base;

namespace TronBox.Domain.InnerClass
{
    public class DadosOrigemDocumentoFiscal
    {
        public OrigemDocumentoFiscal Origem { get; set; }
        public string Originador { get; set; }
    }

    public class DadosOrigemDocumentoFiscalValidator : AbstractValidator<DadosOrigemDocumentoFiscal>
    {
        public DadosOrigemDocumentoFiscalValidator()
        {
            RuleFor(a => a.Origem)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Origem"));

            RuleFor(a => a.Originador)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Originador"));
        }
    }
}
