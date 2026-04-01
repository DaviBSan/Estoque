using Estoque.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class UsuariosController : Controller
    {
        private EstoqueContext db = new EstoqueContext();

        public ActionResult Index()
        {
            return View(db.Usuarios.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Usuario usuario)
        {
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
            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();
            return View(usuario);
        }

        [HttpPost]
        public ActionResult Edit(Usuario usuario)
        {
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
            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
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