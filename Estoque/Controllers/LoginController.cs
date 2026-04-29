
using Estoque.Models;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class LoginController : Controller
    {
        private EstoqueContext db = new EstoqueContext();

        public ActionResult Index()
        {
            if (Session["Admin"] != null)
                return RedirectToAction("Index", "Produtos");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string NomeUsuario, string Senha)
        {
            if (string.IsNullOrWhiteSpace(NomeUsuario) || string.IsNullOrWhiteSpace(Senha))
            {
                ViewBag.Erro = "Preencha todos os campos!";
                return View();
            }

            try
            {
                var admin = db.Admins.FirstOrDefault(a =>
                    a.NomeUsuario == NomeUsuario && a.Senha == Senha);

                if (admin != null)
                {
                    Session["Admin"] = admin.NomeUsuario;
                    Session["AdminId"] = admin.Id;
                    return RedirectToAction("Index", "Produtos");
                }

                ViewBag.Erro = "Usuário ou senha incorretos!";
                return View();
            }
            catch
            {
                ViewBag.Erro = "Erro ao acessar o sistema. Tente novamente.";
                return View();
            }
        }

        public ActionResult Registro()
        {
            if (Session["Admin"] != null)
                return RedirectToAction("Index", "Produtos");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(string NomeUsuario, string Senha, string ConfirmarSenha)
        {
            if (string.IsNullOrWhiteSpace(NomeUsuario) || string.IsNullOrWhiteSpace(Senha))
            {
                ViewBag.Erro = "Preencha todos os campos!";
                return View();
            }

            if (Senha != ConfirmarSenha)
            {
                ViewBag.Erro = "As senhas não coincidem!";
                return View();
            }

            try
            {
                bool existe = db.Admins.Any(a => a.NomeUsuario == NomeUsuario);
                if (existe)
                {
                    ViewBag.Erro = "Esse nome de usuário já está em uso!";
                    return View();
                }

                var novoAdmin = new Admin
                {
                    NomeUsuario = NomeUsuario,
                    Senha = Senha
                };

                db.Admins.Add(novoAdmin);
                db.SaveChanges();

                TempData["Sucesso"] = "Conta criada com sucesso! Faça o login.";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Erro = "Erro ao criar conta. Tente novamente.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
