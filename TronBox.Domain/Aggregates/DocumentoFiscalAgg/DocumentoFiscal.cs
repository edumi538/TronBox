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
        [BsonIgnoreIfDefault]
        public string InscricaoEstadual { get; set; }
        public ETipoDocumentoFiscal TipoDocumentoFiscal { get; set; }
        public string ChaveDocumentoFiscal { get; set; }
        public string NumeroDocumentoFiscal { get; set; }
        [BsonIgnoreIfDefault]
        public double ValorDocumentoFiscal { get; set; }
        [BsonIgnoreIfDefault]
        public string SerieDocumentoFiscal { get; set; }
        [BsonIgnoreIfDefault]
        public string NsuDocumentoFiscal { get; set; }
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
                    case ETipoDocumentoFiscal.NfeEntrada:
                        return "NFe";
                    case ETipoDocumentoFiscal.NfeSaida:
                        return "NFe";
                    case ETipoDocumentoFiscal.CteEntrada:
                        return "CTe";
                    case ETipoDocumentoFiscal.CteSaida:
                        return "CTe";
                    case ETipoDocumentoFiscal.Nfce:
                        return "NFCe";
                    case ETipoDocumentoFiscal.NfseEntrada:
                        return "NFse";
                    case ETipoDocumentoFiscal.NfseSaida:
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
                if (TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseSaida || TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseEntrada)
                    return $"{Constantes.URL_AZURE}/box/documentosfiscais/nfse/{DataEmissaoDocumento.ToString().Substring(2, 4)}/{ChaveDocumentoFiscal}";

                var tipo = "cte";
                var modelo = ChaveDocumentoFiscal.Substring(20, 2);

                if (modelo == "55") tipo = "nfe";

                if (modelo == "65") tipo = "nfce";

                var anomes = ChaveDocumentoFiscal.Substring(2, 4);

                return $"{Constantes.URL_AZURE}/box/documentosfiscais/{tipo}/{anomes}/{ChaveDocumentoFiscal}";
            }
        }
        [BsonIgnore]
        public string NomeArquivo { get; set; }
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
