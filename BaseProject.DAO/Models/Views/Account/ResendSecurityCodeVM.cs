using System.ComponentModel.DataAnnotations;

namespace BaseProject.DAO.Models.Views
{
	public class ResendSecurityCodeVM
	{
		[Required()]
		public string UserName { get; set; }
	}
}
