using System.ComponentModel.DataAnnotations;

namespace BaseProject.DAO.Models.Views
{
    public class EmailConfirmVM
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [EmailAddress(ErrorMessage = "O email digitado não é válido!")]
        public string Email { get; set; }
        public bool IsConfirmed { get; set; }
        public bool EmailSent { get; set; }
        public bool EmailVerified { get; set; }
    }
}
