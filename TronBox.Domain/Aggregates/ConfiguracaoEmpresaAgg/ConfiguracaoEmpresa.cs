﻿using Comum.Domain.Enums;
using FluentValidation;
using System.Collections.Generic;
using TronBox.Domain.Enums;
using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg
{
    public class ConfiguracaoEmpresa : Entity<ConfiguracaoEmpresa>
    {
        public string Inscricao { get; set; }
        public bool PermiteEmpresaEditarPerfil { get; set; }
        public bool PermiteDownloadSemManifesto { get; set; }
        public ArquiteturaDownload ArquiteturaDownload { get; set; }
        public DadosMatoGrosso DadosMatoGrosso { get; set; }
        public IEnumerable<InscricaoComplementar> InscricoesComplementares { get; set; }
    }

    #region Dados Complementares
    public class DadosMatoGrosso
    {
        public TipoAcessoMatoGrosso Tipo { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }

    public class InscricaoComplementar
    {
        public bool ConsultaMatoGrosso { get; set; }
        public eSituacao Situacao { get; set; }
        public string InscricaoEstadual { get; set; }
        public bool SalvarCteEntrada { get; set; }
        public bool SalvarCteSaida { get; set; }
        public string UltimoNsuNfe { get; set; }
        public string UltimoNsuCTe { get; set; }
        public string ErroConsulta { get; set; }
        public DadosIcmsTransparente DadosIcmsTransparente { get; set; }
        public IEnumerable<DadosMunicipais> DadosMunicipais { get; set; }
    }

    public class DadosIcmsTransparente
    {
        public string CodigoAcesso { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }

    public class DadosMunicipais
    {
        public string InscricaoMunicipal { get; set; }
        public eSituacao Situacao { get; set; }
    }
    #endregion

    public class ConfiguracaoEmpresaValidator : AbstractValidator<ConfiguracaoEmpresa>
    {
        public ConfiguracaoEmpresaValidator()
        {
            #region Validações
            RuleFor(a => a.Inscricao)
               .NotEmpty().WithMessage(MensagensValidacao.Requerido("Inscrição"));

            RuleFor(a => a.ArquiteturaDownload)
                .NotEmpty().WithMessage(MensagensValidacao.Requerido("Arquitetura de Download"));
            #endregion
        }
    }
}
