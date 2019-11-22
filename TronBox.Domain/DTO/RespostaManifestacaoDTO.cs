namespace TronBox.Domain.DTO
{
    public class RespostaManifestacaoDTO
    {
        public bool Success { get; set; }
        public DataDTO Data { get; set; }

        public class DataDTO
        {
            public EventoDTO InfEvento { get; set; }

            public class EventoDTO
            {
                public string XMotivo { get; set; }
                public string CStat { get; set; }
                public string ChNFe { get; set; }
            }
        }
    }
}
