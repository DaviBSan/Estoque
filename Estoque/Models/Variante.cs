using Estoque.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class Variante
    {
        public int Id { get; set; }

        // Chave estrangeira para Produto
        public int ProdutoId { get; set; }

        [Required]
        [Display(Name = "Tamanho")]
        public string Tamanho { get; set; } // PP, P, M, G, GG

        [Required]
        [Display(Name = "Cor")]
        public string Cor { get; set; }

        [Display(Name = "Qtd. em Estoque")]
        public int QuantidadeEmEstoque { get; set; }

        // Propriedade de navegação (EF vai buscar o Produto relacionado)
        public virtual Produto Produto { get; set; }

        public virtual ICollection<ItemVenda> ItensVenda { get; set; }
        public virtual ICollection<MovimentacaoEstoque> Movimentacoes { get; set; }
    }
}