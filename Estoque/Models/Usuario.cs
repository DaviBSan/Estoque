using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        public string Telefone { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public virtual ICollection<Venda> Vendas { get; set; }
    }
}