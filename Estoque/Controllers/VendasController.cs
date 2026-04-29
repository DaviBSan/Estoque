
using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class VendasController : Controller
    {
        private EstoqueContext db = new EstoqueContext();

        private bool AdminLogado() => Session["Admin"] != null;

        // ─── INDEX ────────────────────────────────────────────────────────────
        public ActionResult Index()
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            try
            {
                var vendas = db.Vendas
                    .Include(v => v.Usuario)
                    .Include(v => v.Itens)
                    .OrderByDescending(v => v.DataVenda)
                    .ToList();

                return View(vendas);
            }
            catch
            {
                ViewBag.Erro = "Erro ao carregar as vendas.";
                return View(new List<Venda>());
            }
        }

        // ─── DETAILS ──────────────────────────────────────────────────────────
        public ActionResult Details(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            try
            {
                var venda = db.Vendas
                    .Include(v => v.Usuario)
                    .Include(v => v.Itens.Select(i => i.Variante.Produto))
                    .FirstOrDefault(v => v.Id == id);

                if (venda == null) return HttpNotFound();

                return View(venda);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // ─── CREATE GET ───────────────────────────────────────────────────────
        public ActionResult Create()
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            try
            {
                ViewBag.UsuarioId = new SelectList(db.Usuarios, "Id", "Nome");
                ViewBag.VarianteId = new SelectList(
                    db.Variantes
                      .Include(v => v.Produto)
                      .Where(v => v.QuantidadeEmEstoque > 0)
                      .ToList()
                      .Select(v => new {
                          v.Id,
                          Nome = $"{v.Produto.Nome} | {v.Tamanho} | {v.Cor} | Estoque: {v.QuantidadeEmEstoque} | R$ {v.Produto.Preco:F2}"
                      }),
                    "Id", "Nome");

                return View();
            }
            catch
            {
                ViewBag.Erro = "Erro ao carregar formulário.";
                return RedirectToAction("Index");
            }
        }

        // ─── CREATE POST ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int UsuarioId, int[] VarianteIds, int[] Quantidades)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            // Validação básica
            if (VarianteIds == null || VarianteIds.Length == 0)
            {
                ViewBag.Erro = "Adicione pelo menos um item à venda.";
                ViewBag.UsuarioId = new SelectList(db.Usuarios, "Id", "Nome", UsuarioId);
                ViewBag.VarianteId = BuildVarianteSelectList();
                return View();
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    decimal valorTotal = 0;
                    var itens = new List<ItemVenda>();

                    for (int i = 0; i < VarianteIds.Length; i++)
                    {
                        int varId = VarianteIds[i];
                        int qtd = Quantidades != null && i < Quantidades.Length ? Quantidades[i] : 1;

                        if (qtd <= 0) continue;

                        // Busca variante e valida estoque
                        var variante = db.Variantes
                            .Include(v => v.Produto)
                            .FirstOrDefault(v => v.Id == varId);

                        if (variante == null)
                            throw new Exception($"Variante {varId} não encontrada.");

                        if (variante.QuantidadeEmEstoque < qtd)
                            throw new Exception(
                                $"Estoque insuficiente para {variante.Produto.Nome} ({variante.Tamanho}/{variante.Cor}). " +
                                $"Disponível: {variante.QuantidadeEmEstoque}.");

                        decimal precoUnit = variante.Produto.Preco;
                        valorTotal += precoUnit * qtd;

                        itens.Add(new ItemVenda
                        {
                            VarianteId = varId,
                            Quantidade = qtd,
                            PrecoUnitario = precoUnit
                        });

                        // Atualiza estoque
                        variante.QuantidadeEmEstoque -= qtd;
                        db.Entry(variante).State = EntityState.Modified;

                        // Registra movimentação
                        db.Movimentacoes.Add(new MovimentacaoEstoque
                        {
                            VarianteId = varId,
                            Tipo = "Saida",
                            Quantidade = qtd,
                            Motivo = "Venda",
                            DataMovimentacao = DateTime.Now
                        });
                    }

                    if (itens.Count == 0)
                        throw new Exception("Nenhum item válido foi adicionado.");

                    // Cria a venda
                    var venda = new Venda
                    {
                        UsuarioId = UsuarioId,
                        DataVenda = DateTime.Now,
                        ValorTotal = valorTotal,
                        Status = "Concluída",
                        Itens = itens
                    };

                    db.Vendas.Add(venda);
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["Sucesso"] = $"Venda #{venda.Id} registrada com sucesso! Total: R$ {valorTotal:F2}";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ViewBag.Erro = ex.Message;
                    ViewBag.UsuarioId = new SelectList(db.Usuarios, "Id", "Nome", UsuarioId);
                    ViewBag.VarianteId = BuildVarianteSelectList();
                    return View();
                }
            }
        }

        // ─── CANCELAR ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancelar(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var venda = db.Vendas
                        .Include(v => v.Itens.Select(i => i.Variante))
                        .FirstOrDefault(v => v.Id == id);

                    if (venda == null) return HttpNotFound();

                    if (venda.Status == "Cancelada")
                        throw new Exception("Esta venda já está cancelada.");

                    // Devolve estoque
                    foreach (var item in venda.Itens)
                    {
                        item.Variante.QuantidadeEmEstoque += item.Quantidade;
                        db.Entry(item.Variante).State = EntityState.Modified;

                        db.Movimentacoes.Add(new MovimentacaoEstoque
                        {
                            VarianteId = item.VarianteId,
                            Tipo = "Entrada",
                            Quantidade = item.Quantidade,
                            Motivo = $"Cancelamento da Venda #{id}",
                            DataMovimentacao = DateTime.Now
                        });
                    }

                    venda.Status = "Cancelada";
                    db.Entry(venda).State = EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["Sucesso"] = $"Venda #{id} cancelada e estoque restaurado.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Erro"] = ex.Message;
                    return RedirectToAction("Index");
                }
            }
        }

        // ─── DELETE ───────────────────────────────────────────────────────────
        public ActionResult Delete(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            try
            {
                var venda = db.Vendas
                    .Include(v => v.Usuario)
                    .FirstOrDefault(v => v.Id == id);

                if (venda == null) return HttpNotFound();
                return View(venda);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");

            try
            {
                var venda = db.Vendas
                    .Include(v => v.Itens)
                    .FirstOrDefault(v => v.Id == id);

                if (venda == null) return HttpNotFound();

                db.ItensVenda.RemoveRange(venda.Itens);
                db.Vendas.Remove(venda);
                db.SaveChanges();

                TempData["Sucesso"] = "Venda excluída com sucesso.";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Erro"] = "Erro ao excluir venda.";
                return RedirectToAction("Index");
            }
        }

        // ─── HELPER ───────────────────────────────────────────────────────────
        private SelectList BuildVarianteSelectList()
        {
            var lista = db.Variantes
                .Include(v => v.Produto)
                .Where(v => v.QuantidadeEmEstoque > 0)
                .ToList()
                .Select(v => new {
                    v.Id,
                    Nome = $"{v.Produto.Nome} | {v.Tamanho} | {v.Cor} | Estoque: {v.QuantidadeEmEstoque} | R$ {v.Produto.Preco:F2}"
                });

            return new SelectList(lista, "Id", "Nome");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
