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
        [BsonIgnoreIfDefault]
        public bool SalvarCteEntrada { get; set; }
        [BsonIgnoreIfDefault]
        public bool SalvarCteSaida { get; set; }
        [BsonIgnoreIfDefault]
        public bool SalvarCteNaoTomador { get; set; }
        [BsonIgnoreIfDefault]
        public bool ManifestarAutomaticamente { get; set; }
        [BsonIgnoreIfDefault]
        public bool SalvarSomenteManifestadas { get; set; }
        [BsonIgnoreIfDefault]
        public EMetodoBusca MetodoBusca { get; set; }
        [BsonIgnoreIfDefault]
        public EEstruturaDownload EstruturaDownload { get; set; }
        [BsonIgnoreIfDefault]
        public DadosMatoGrosso DadosMatoGrosso { get; set; }
        [BsonIgnoreIfDefault]
        public DadosMatoGrossoSul DadosMatoGrossoSul { get; set; }
        [BsonIgnoreIfDefault]
        public IEnumerable<InscricaoComplementar> InscricoesComplementares { get; set; }
    }

    public class DadosMatoGrosso
    {
        public ETipoAcessoMatoGrosso Tipo { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }

    public class DadosMatoGrossoSul
    {
        public string Usuario { get; set; }
        public string CodigoAcesso { get; set; }
        public string Senha { get; set; }
    }

    public class InscricaoComplementar : Entity<InscricaoComplementar>
    {
        [BsonIgnoreIfNull]
        public string NomeFantasia { get; set; }
        [BsonIgnoreIfNull]
        public string Telefone { get; set; }
        [BsonIgnoreIfNull]
        public string Celular { get; set; }
        [BsonIgnoreIfNull]
        public string InscricaoEstadual { get; set; }
        [BsonIgnoreIfNull]
        public string InscricaoMunicipal { get; set; }
        [BsonIgnoreIfNull]
        public string Logradouro { get; set; }
        [BsonIgnoreIfNull]
        public string Numero { get; set; }
        [BsonIgnoreIfNull]
        public string Complemento { get; set; }
        [BsonIgnoreIfNull]
        public string Bairro { get; set; }
        [BsonIgnoreIfNull]
        public string Cep { get; set; }
        [BsonIgnoreIfDefault]
        public int CodigoCidade { get; set; }
        [BsonIgnoreIfNull]
        public string UF { get; set; }
        [BsonIgnoreIfDefault]
        public bool ConsultaPortalEstadual { get; set; }
        [BsonIgnoreIfDefault]
        public eSituacao Situacao { get; set; }
    }

    public class ConfiguracaoEmpresaValidator : AbstractValidator<ConfiguracaoEmpresa>
    {
        public ConfiguracaoEmpresaValidator()
        {
            RuleFor(a => a.InscricoesComplementares)
               .NotNull().WithMessage(MensagensValidacao.Requerido("Inscrições Complementares"));
        }
    }
}
