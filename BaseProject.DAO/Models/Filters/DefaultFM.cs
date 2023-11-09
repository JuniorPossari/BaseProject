
namespace BaseProject.DAO.Models.Filters
{
	public class DefaultFM
	{
		public int? IdEntidade { get; set; }
		public int? Status { get; set; }
		public string Texto { get; set; }
		public decimal? Valor { get; set; }
		public decimal? ValorInicio { get; set; }
		public decimal? ValorFim { get; set; }
		public DateTime? Data { get; set; }
		public DateTime? DataInicio { get; set; }
		public DateTime? DataFim { get; set; }		
		public bool? Veridico { get; set; }
	}
}
