using Estoque.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class Venda
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [Display(Name = "Data da Venda")]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Display(Name = "Valor Total")]
        public decimal ValorTotal { get; set; }

        public string Status { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<ItemVenda> Itens { get; set; }
    }
}