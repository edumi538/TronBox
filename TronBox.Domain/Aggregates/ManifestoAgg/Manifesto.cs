using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using TronBox.Domain.Enums;
using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.ManifestoAgg
{
    public class Manifesto : Entity<Manifesto>
    {
        public string ChaveDocumentoFiscal { get; set; }
        public string NumeroDocumentoFiscal { get; set; }
        public double ValorDocumentoFiscal { get; set; }
        public int DataArmazenamento { get; set; }
        public int DataEmissaoManifesto { get; set; }
        [BsonIgnoreIfDefault]
        public int DataManifesto { get; set; }
        public SituacaoManifesto SituacaoManifesto { get; set; }
        public SituacaoDocumentoFiscal SituacaoDocumentoFiscal { get; set; }
        public bool Cancelado { get; set; }
        public bool Rejeitado { get; set; }
        public DadosOrigem DadosOrigem { get; set; }
        [BsonIgnoreIfDefault]
        public DadosFornecedor DadosFornecedor { get; set; }
        [BsonIgnoreIfDefault]
        public DadosRetorno DadosManifestacao { get; set; }
        [BsonIgnoreIfDefault]
        public DadosRetorno DadosDownload { get; set; }
    }

    public class DadosOrigem
    {
        public OrigemManifesto Origem { get; set; }
        public string Originador { get; set; }
    }

    public class DadosFornecedor
    {
        public string Inscricao { get; set; }
        public string RazaoSocial { get; set; }
    }

    public class DadosRetorno
    {
        public string CodigoRetorno { get; set; }
        public string Motivo { get; set; }
        public bool Rejeitado { get; set; }
    }

    public class ManifestoValidator : AbstractValidator<Manifesto>
    {
        public ManifestoValidator()
        {
            RuleFor(a => a.ChaveDocumentoFiscal)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Chave do Documento Fiscal"));

            RuleFor(a => a.NumeroDocumentoFiscal)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Número do Documento Fiscal"));

            RuleFor(a => a.ValorDocumentoFiscal)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Valor do Documento Fiscal"))
                .NotEqual(0).WithMessage(MensagensValidacao.NaoPodeSerIgualA("Valor do Documento Fiscal", "zero"));

            RuleFor(a => a.DataArmazenamento)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Data de Armazenamento"));

            RuleFor(a => a.DataEmissaoManifesto)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Data de Emissão"));

            RuleFor(a => a.SituacaoManifesto)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Situação do Manifesto"));

            RuleFor(a => a.SituacaoDocumentoFiscal)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Situação do Documento Fiscal"));

            RuleFor(a => a.DadosOrigem)
                .NotNull().WithMessage(MensagensValidacao.Requerido("Dados da Origem"));

            RuleFor(a => a.DadosFornecedor)
                .NotNull().WithMessage(MensagensValidacao.Requerido("Dados do Fornecedor"));
        }
    }
}
