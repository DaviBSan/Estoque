namespace Estoque.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoInicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemVendas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VendaId = c.Int(nullable: false),
                        VarianteId = c.Int(nullable: false),
                        Quantidade = c.Int(nullable: false),
                        PrecoUnitario = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Variantes", t => t.VarianteId, cascadeDelete: true)
                .ForeignKey("dbo.Vendas", t => t.VendaId, cascadeDelete: true)
                .Index(t => t.VendaId)
                .Index(t => t.VarianteId);
            
            CreateTable(
                "dbo.Variantes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProdutoId = c.Int(nullable: false),
                        Tamanho = c.String(nullable: false),
                        Cor = c.String(nullable: false),
                        QuantidadeEmEstoque = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Produtoes", t => t.ProdutoId, cascadeDelete: true)
                .Index(t => t.ProdutoId);
            
            CreateTable(
                "dbo.MovimentacaoEstoques",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VarianteId = c.Int(nullable: false),
                        Tipo = c.String(nullable: false),
                        Quantidade = c.Int(nullable: false),
                        Motivo = c.String(),
                        DataMovimentacao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Variantes", t => t.VarianteId, cascadeDelete: true)
                .Index(t => t.VarianteId);
            
            CreateTable(
                "dbo.Produtoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Descricao = c.String(),
                        Preco = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Genero = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Vendas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioId = c.Int(nullable: false),
                        DataVenda = c.DateTime(nullable: false),
                        ValorTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuarios", t => t.UsuarioId, cascadeDelete: true)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Email = c.String(),
                        Telefone = c.String(),
                        DataCadastro = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vendas", "UsuarioId", "dbo.Usuarios");
            DropForeignKey("dbo.ItemVendas", "VendaId", "dbo.Vendas");
            DropForeignKey("dbo.Variantes", "ProdutoId", "dbo.Produtoes");
            DropForeignKey("dbo.MovimentacaoEstoques", "VarianteId", "dbo.Variantes");
            DropForeignKey("dbo.ItemVendas", "VarianteId", "dbo.Variantes");
            DropIndex("dbo.Vendas", new[] { "UsuarioId" });
            DropIndex("dbo.MovimentacaoEstoques", new[] { "VarianteId" });
            DropIndex("dbo.Variantes", new[] { "ProdutoId" });
            DropIndex("dbo.ItemVendas", new[] { "VarianteId" });
            DropIndex("dbo.ItemVendas", new[] { "VendaId" });
            DropTable("dbo.Usuarios");
            DropTable("dbo.Vendas");
            DropTable("dbo.Produtoes");
            DropTable("dbo.MovimentacaoEstoques");
            DropTable("dbo.Variantes");
            DropTable("dbo.ItemVendas");
        }
    }
}
