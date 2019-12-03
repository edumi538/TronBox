using Comum.Domain.Enums;
using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using TronBox.Domain.Enums;
using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg
{
    public class ConfiguracaoEmpresa : Entity<ConfiguracaoEmpresa>
    {
        public string Inscricao { get; set; }
        [BsonIgnoreIfDefault]
        public bool SalvarCteEntrada { get; set; }
        [BsonIgnoreIfDefault]
        public bool SalvarCteSaida { get; set; }
        [BsonIgnoreIfDefault]
        public bool ManifestarAutomaticamente { get; set; }
        [BsonIgnoreIfDefault]
        public bool SalvarSomenteManifestadas { get; set; }
        [BsonIgnoreIfDefault]
        public EMetodoBusca MetodoBusca { get; set; }
        [BsonIgnoreIfDefault]
        public DadosMatoGrosso DadosMatoGrosso { get; set; }
        [BsonIgnoreIfDefault]
        public IEnumerable<InscricaoComplementar> InscricoesComplementares { get; set; }
    }

    public class DadosMatoGrosso
    {
        public ETipoAcessoMatoGrosso Tipo { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }

    public class InscricaoComplementar : Entity<InscricaoComplementar>
    {
        [BsonIgnoreIfNull]
        public string InscricaoEstadual { get; set; }
        [BsonIgnoreIfNull]
        public string InscricaoMunicipal { get; set; }
        [BsonIgnoreIfNull]
        public bool ConsultaMatoGrosso { get; set; }
        [BsonIgnoreIfNull]
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
