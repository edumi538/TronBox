using System;
using TronCore.Utilitarios;

namespace TronBox.Domain.DTO
{
    public class DadosComputadorMonitorDTO
    {
        public string Id { get; set; }
        public long DataHora { get; set; }
        public DateTime DataHoraFormatada { get => UtilitarioDatas.ConvertIntToDateTime(DataHora); }
        public string Usuario { get; set; }
        public string SistemaOperacional { get; set; }
        public string VersaoSistemaOperacional { get; set; }
        public int ArquiteturaSistemaOperacional { get; set; }
        public string TipoSistemaOperacional { get => $"Sistema Operacional de {ArquiteturaSistemaOperacional} bits."; }
        public string NomeComputador { get; set; }
        public string NomeUsuario { get; set; }
        public string Processador { get; set; }
        public int ArquiteturaProcessador { get; set; }
        public string TipoProcessador { get => $"Prcessador com base em x{ArquiteturaProcessador}"; }
        public long MemoriaRam { get; set; }
        public string QuantidadeMemoriaRam { get => $"{Math.Round((double)(MemoriaRam / 1024) / 1024)}GB"; }
    }
}
