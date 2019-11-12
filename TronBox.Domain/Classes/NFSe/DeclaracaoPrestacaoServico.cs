using System;

namespace TronBox.Domain.Classes.NFSe
{
    public class DeclaracaoPrestacaoServico
    {
        public InfDeclaracaoPrestacaoServico InfDeclaracaoPrestacaoServico { get; set; }
    }

    public class InfDeclaracaoPrestacaoServico : IDisposable
    {
        public Servico Servico { get; set; }
        public Prestador Prestador { get; set; }
        public Tomador Tomador { get; set; }

        public void Dispose()
        {
        }
    }
}
