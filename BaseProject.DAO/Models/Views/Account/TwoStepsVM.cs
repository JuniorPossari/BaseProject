using System.ComponentModel.DataAnnotations;

namespace BaseProject.DAO.Models.Views
{
	public class TwoStepsVM
	{
		[Required()]
		public string UserName { get; set; }
		[Required()]
		[StringLength(6, MinimumLength = 6)]
		public string Code { get; set; }
	}
}
