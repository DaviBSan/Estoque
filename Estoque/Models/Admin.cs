using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório")]
        [Display(Name = "Usuário")]
        public string NomeUsuario { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [Display(Name = "Senha")]
        public string Senha { get; set; }
    }
}