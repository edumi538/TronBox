using Comum.Domain.Enums;
using FluentValidation;
using System.Collections.Generic;
using TronBox.Domain.Enums;
using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg
{
    public class ConfiguracaoEmpresa : Entity<ConfiguracaoEmpresa>
    {
        public string Inscricao { get; set; }
        public bool SalvarCteEntrada { get; set; }
        public bool SalvarCteSaida { get; set; }
        public bool ManifestarAutomaticamente { get; set; }
        public EMetodoBusca MetodoBusca { get; set; }
        public string UltimoNsuNfe { get; set; }
        public string UltimoNsuCte { get; set; }
        public DadosMatoGrosso DadosMatoGrosso { get; set; }
        public IEnumerable<InscricaoComplementar> InscricoesComplementares { get; set; }
    }

    public class DadosMatoGrosso
    {
        public ETipoAcessoMatoGrosso Tipo { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }

    public class InscricaoComplementar
    {
        public bool ConsultaMatoGrosso { get; set; }
        public eSituacao Situacao { get; set; }
        public string InscricaoEstadual { get; set; }
        public IEnumerable<DadosMunicipais> DadosMunicipais { get; set; }
    }

    public class DadosMunicipais
    {
        public string InscricaoMunicipal { get; set; }
        public eSituacao Situacao { get; set; }
    }

    public class ConfiguracaoEmpresaValidator : AbstractValidator<ConfiguracaoEmpresa>
    {
        public ConfiguracaoEmpresaValidator()
        {
            RuleFor(a => a.Inscricao)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Inscrição"));
        }
    }
}
