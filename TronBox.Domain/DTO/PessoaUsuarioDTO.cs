using Comum.Domain.Enums;
using System;

namespace TronBox.Domain.DTO
{
    public class PessoaUsuarioDTO
    {
        public eClassificacaoPessoa Tipo { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid EmpresaId { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string TelefoneFixo { get; set; }
        public string TelefoneCelular { get; set; }
    }
}
