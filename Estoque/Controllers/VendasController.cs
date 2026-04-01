using Estoque.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class VendasController : Controller
    {
        private EstoqueContext db = new EstoqueContext();

        public ActionResult Index()
        {
            var vendas = db.Vendas.Include(v => v.Usuario).ToList();
            return View(vendas);
        }

        public ActionResult Create()
        {
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "Id", "Nome");
            ViewBag.VarianteId = new SelectList(
                db.Variantes.Include(v => v.Produto)
                .Select(v => new {
                    v.Id,
                    Nome = v.Produto.Nome + " - " + v.Cor + " - " + v.Tamanho
                }), "Id", "Nome");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Venda venda, int VarianteId, int Quantidade)
        {
            var variante = db.Variantes.Find(VarianteId);

            if (variante == null || variante.QuantidadeEmEstoque < Quantidade)
            {
                ModelState.AddModelError("", "Estoque insuficiente para esta variante!");
                ViewBag.UsuarioId = new SelectList(db.Usuarios, "Id", "Nome");
                ViewBag.VarianteId = new SelectList(
                    db.Variantes.Include(v => v.Produto)
                    .Select(v => new {
                        v.Id,
                        Nome = v.Produto.Nome + " - " + v.Cor + " - " + v.Tamanho
                    }), "Id", "Nome");
                return View(venda);
            }

            if (ModelState.IsValid)
            {
                var item = new ItemVenda
                {
                    VarianteId = VarianteId,
                    Quantidade = Quantidade,
                    PrecoUnitario = variante.Produto.Preco
                };

                venda.DataVenda = DateTime.Now;
                venda.Status = "Concluída";
                venda.ValorTotal = item.PrecoUnitario * Quantidade;
                venda.Itens = new System.Collections.Generic.List<ItemVenda> { item };

                variante.QuantidadeEmEstoque -= Quantidade;

                var movimentacao = new MovimentacaoEstoque
                {
                    VarianteId = VarianteId,
                    Tipo = "Saida",
                    Quantidade = Quantidade,
                    Motivo = "Venda",
                    DataMovimentacao = DateTime.Now
                };

                db.Vendas.Add(venda);
                db.Movimentacoes.Add(movimentacao);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(venda);
        }

        public ActionResult Details(int id)
        {
            var venda = db.Vendas
                .Include(v => v.Usuario)
                .Include(v => v.Itens.Select(i => i.Variante.Produto))
                .FirstOrDefault(v => v.Id == id);

            if (venda == null) return HttpNotFound();
            return View(venda);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}