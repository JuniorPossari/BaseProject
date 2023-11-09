using System.ComponentModel.DataAnnotations;

namespace BaseProject.DAO.Models.Views
{
	public class ChangePasswordVM
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha atual")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[$*&@#!?%\-\=\\\/\[\]\{\}\(\)])[0-9a-zA-Z$*&@#!?%\-\=\\\/\[\]\{\}\(\)]{8,16}$", ErrorMessage = "A senha deve ter:<br /> <li>Entre 8 e 16 caracteres</li> <li>Pelo menos 1 letra maiúscula e minúscula</li> <li>Pelo menos 1 número e 1 símbolo</li>")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme a nova senha")]
        [Compare("NewPassword", ErrorMessage = "A nova senha e a confirmação da nova senha não conferem!")]
        public string ConfirmNewPassword { get; set; }
    }

}
