using System.ComponentModel.DataAnnotations;

namespace BaseProject.DAO.Models.Views
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [EmailAddress(ErrorMessage = "O email digitado não é válido!")]
        public string Email { get; set; }
    }
}
