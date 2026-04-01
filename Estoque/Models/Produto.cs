using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Preço")]
        public decimal Preco { get; set; }

        [Display(Name = "Gênero")]
        public string Genero { get; set; } // Ex: "Masculino", "Feminino", "Unissex"

        // Relacionamento: um produto tem várias variantes
        public virtual ICollection<Variante> Variantes { get; set; }
    }
}