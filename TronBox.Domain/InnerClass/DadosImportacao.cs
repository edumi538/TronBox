using FluentValidation;
using TronCore.Dominio.Base;

namespace TronBox.Domain.InnerClass
{
    public class DadosImportacao
    {
        public long DataImportacao { get; set; }
        public string Usuario { get; set; }
    }

    public class DadosImportacaoValidator : AbstractValidator<DadosImportacao>
    {
        public DadosImportacaoValidator()
        {
            RuleFor(a => a.DataImportacao)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Data de Importação"));

            RuleFor(a => a.Usuario)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Usuário"));
        }
    }
}
