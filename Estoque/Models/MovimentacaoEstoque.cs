using System;
using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }

        public int VarianteId { get; set; }

        // "Entrada" ou "Saida"
        [Required]
        public string Tipo { get; set; }

        [Required]
        public int Quantidade { get; set; }

        public string Motivo { get; set; } // Ex: "Compra de fornecedor", "Venda", "Ajuste"

        [Display(Name = "Data")]
        public DateTime DataMovimentacao { get; set; } = DateTime.Now;

        public virtual Variante Variante { get; set; }
    }
}