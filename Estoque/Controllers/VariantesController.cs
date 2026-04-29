
using Estoque.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class VariantesController : Controller
    {
        private EstoqueContext db = new EstoqueContext();

        private bool AdminLogado() => Session["Admin"] != null;

        // ─── INDEX ────────────────────────────────────────────────────────────
        public ActionResult Index()
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            try
            {
                var variantes = db.Variantes
                    .Include(v => v.Produto)
                    .OrderBy(v => v.Produto.Nome)
                    .ToList();

                return View(variantes);
            }
            catch
            {
                ViewBag.Erro = "Erro ao carregar variantes.";
                return View(new System.Collections.Generic.List<Variante>());
            }
        }

        // ─── CREATE GET ───────────────────────────────────────────────────────
        public ActionResult Create()
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            ViewBag.ProdutoId = new SelectList(db.Produtos, "Id", "Nome");
            return View();
        }

        // ─── CREATE POST ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Variante variante)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                try
                {
                    db.Variantes.Add(variante);
                    db.SaveChanges();

                    // Registra movimentação de entrada inicial
                    if (variante.QuantidadeEmEstoque > 0)
                    {
                        db.Movimentacoes.Add(new MovimentacaoEstoque
                        {
                            VarianteId = variante.Id,
                            Tipo = "Entrada",
                            Quantidade = variante.QuantidadeEmEstoque,
                            Motivo = "Estoque inicial",
                            DataMovimentacao = System.DateTime.Now
                        });
                        db.SaveChanges();
                    }

                    TempData["Sucesso"] = "Variante criada com sucesso!";
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.Erro = "Erro ao salvar variante. Tente novamente.";
                }
            }

            ViewBag.ProdutoId = new SelectList(db.Produtos, "Id", "Nome", variante.ProdutoId);
            return View(variante);
        }

        // ─── EDIT GET ─────────────────────────────────────────────────────────
        public ActionResult Edit(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            try
            {
                var variante = db.Variantes.Find(id);
                if (variante == null) return HttpNotFound();

                ViewBag.ProdutoId = new SelectList(db.Produtos, "Id", "Nome", variante.ProdutoId);
                return View(variante);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // ─── EDIT POST ────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Variante variante)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(variante).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    TempData["Sucesso"] = "Variante atualizada com sucesso!";
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.Erro = "Erro ao atualizar variante.";
                }
            }

            ViewBag.ProdutoId = new SelectList(db.Produtos, "Id", "Nome", variante.ProdutoId);
            return View(variante);
        }

        // ─── DELETE GET ───────────────────────────────────────────────────────
        public ActionResult Delete(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            try
            {
                var variante = db.Variantes
                    .Include(v => v.Produto)
                    .FirstOrDefault(v => v.Id == id);

                if (variante == null) return HttpNotFound();
                return View(variante);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // ─── DELETE POST ──────────────────────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            try
            {
                var variante = db.Variantes.Find(id);
                if (variante == null) return HttpNotFound();

                db.Variantes.Remove(variante);
                db.SaveChanges();

                TempData["Sucesso"] = "Variante excluída com sucesso!";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Erro"] = "Erro ao excluir variante.";
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
