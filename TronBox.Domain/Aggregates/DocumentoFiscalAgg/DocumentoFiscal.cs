using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using TronBox.Domain.Enums;
using TronBox.Domain.InnerClass;
using TronCore.DefinicoesConfiguracoes;
using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.DocumentoFiscalAgg
{
    public class DocumentoFiscal : Entity<DocumentoFiscal>
    {
        public TipoDocumentoFiscal TipoDocumentoFiscal { get; set; }
        public string ChaveDocumentoFiscal { get; set; }
        public string NumeroDocumentoFiscal { get; set; }
        public double ValorDocumentoFiscal { get; set; }
        public string SerieDocumentoFiscal { get; set; }
        public int DataArmazenamento { get; set; }
        public int DataEmissaoDocumento { get; set; }
        public bool Cancelado { get; set; }
        public bool Rejeitado { get; set; }
        public bool Denegada { get; set; }
        public DadosOrigemDocumentoFiscal DadosOrigem { get; set; }
        [BsonIgnoreIfDefault]
        public DadosImportacao DadosImportacao { get; set; }
        public DadosFornecedor DadosEmitenteDestinatario { get; set; }
        [BsonIgnore]
        public string TipoDocumento
        {
            get
            {
                switch (TipoDocumentoFiscal)
                {
                    case TipoDocumentoFiscal.NfeEntrada:
                        return "NFe";
                    case TipoDocumentoFiscal.NfeSaida:
                        return "NFe";
                    case TipoDocumentoFiscal.CteEntrada:
                        return "CTe";
                    case TipoDocumentoFiscal.CteSaida:
                        return "CTe";
                    case TipoDocumentoFiscal.Nfce:
                        return "NFCe";
                    case TipoDocumentoFiscal.NfseEntrada:
                        return "NFse";
                    case TipoDocumentoFiscal.NfseSaida:
                        return "NFse";
                    default:
                        return "";
                }
            }
        }
        [BsonIgnore]
        public string CaminhoArquivo
        {
            get
            {
                var tipo = "cte";
                var modelo = ChaveDocumentoFiscal.Substring(20, 2);

                if (modelo == "55") tipo = "nfe";

                if (modelo == "65") tipo = "nfce";

                var anomes = ChaveDocumentoFiscal.Substring(2, 4);

                return $"{Constantes.URL_AZURE}/box/documentosfiscais/{tipo}/{anomes}/{ChaveDocumentoFiscal}";
            }
        }
    }

    public class DocumentoFiscalValidator : AbstractValidator<DocumentoFiscal>
    {
        public DocumentoFiscalValidator()
        {
            RuleFor(a => a.TipoDocumentoFiscal)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Tipo do Documento Fiscal"));

            RuleFor(a => a.ChaveDocumentoFiscal)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Chave do Documento Fiscal"));

            RuleFor(a => a.NumeroDocumentoFiscal)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Número do Documento Fiscal"));

            RuleFor(a => a.ValorDocumentoFiscal)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Valor do Documento Fiscal"))
                .NotEqual(0).WithMessage(MensagensValidacao.NaoPodeSerIgualA("Valor do Documento Fiscal", "zero"));

            RuleFor(a => a.SerieDocumentoFiscal)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Série do Documento Fiscal"));

            RuleFor(a => a.DataArmazenamento)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Data de Armazenamento"));

            RuleFor(a => a.DataEmissaoDocumento)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Data de Emissão do Documento"));

            RuleFor(a => a.DadosOrigem)
                .NotNull().WithMessage(MensagensValidacao.Requerido("Dados de Origem"));

            RuleFor(a => a.DadosEmitenteDestinatario)
                .NotNull().WithMessage(MensagensValidacao.Requerido("Dados do Emitente ou Destinatario"));
        }

    }
}
