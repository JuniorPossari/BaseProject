using System.ComponentModel.DataAnnotations;

namespace BaseProject.DAO.Models.Views
{
    public class SignInVM
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [EmailAddress(ErrorMessage = "O email digitado não é válido!")]
        public string Email { get; set; }
        
        [Display(Name = "Senha")]
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
