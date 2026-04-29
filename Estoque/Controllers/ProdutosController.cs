using Estoque.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class ProdutosController : Controller
    {
        private EstoqueContext db = new EstoqueContext();

        private bool AdminLogado()
        {
            return Session["Admin"] != null;
        }

        public ActionResult Index()
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            return View(db.Produtos.Include(p => p.Variantes).ToList());
        }

        public ActionResult Create()
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Produto produto)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            if (ModelState.IsValid)
            {
                db.Produtos.Add(produto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(produto);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}