using Estoque.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class ProdutosController : Controller
    {
        private EstoqueContext db = new EstoqueContext();

        public ActionResult Index()
        {
            return View(db.Produtos.Include(p => p.Variantes).ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Produto produto)
        {
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