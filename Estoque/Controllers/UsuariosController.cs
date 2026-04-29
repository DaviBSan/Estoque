using Estoque.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class UsuariosController : Controller
    {
        private EstoqueContext db = new EstoqueContext();

        private bool AdminLogado()
        {
            return Session["Admin"] != null;
        }

        public ActionResult Index()
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            return View(db.Usuarios.ToList());
        }

        public ActionResult Create()
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Usuario usuario)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        public ActionResult Edit(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();
            return View(usuario);
        }

        [HttpPost]
        public ActionResult Edit(Usuario usuario)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        public ActionResult Delete(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            var usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            if (!AdminLogado()) return RedirectToAction("Index", "Login");
            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();
            return View(usuario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}