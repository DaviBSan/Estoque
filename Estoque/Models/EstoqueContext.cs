using Estoque.Models;
using System.Data.Entity;

namespace Estoque.Models
{
    public class EstoqueContext : DbContext
    {
        public EstoqueContext() : base("EstoqueDB") { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Variante> Variantes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<MovimentacaoEstoque> Movimentacoes { get; set; }
    }
}