using FluentValidation;
using TronCore.Dominio.Base;

namespace TronBox.Domain.InnerClass
{
    public class DadosFornecedor
    {
        public string Inscricao { get; set; }
        public string RazaoSocial { get; set; }
    }

    public class DadosFornecedorValidator : AbstractValidator<DadosFornecedor>
    {
        public DadosFornecedorValidator()
        {
            RuleFor(a => a.RazaoSocial)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Razão Social"));
        }
    }
}
