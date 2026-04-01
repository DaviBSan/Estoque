using Estoque.Models;
using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class ItemVenda
    {
        public int Id { get; set; }

        public int VendaId { get; set; }
        public int VarianteId { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Display(Name = "Preço Unitário")]
        public decimal PrecoUnitario { get; set; }

        public virtual Venda Venda { get; set; }
        public virtual Variante Variante { get; set; }
    }
}