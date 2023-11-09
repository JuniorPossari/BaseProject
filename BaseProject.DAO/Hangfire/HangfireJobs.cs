using BaseProject.DAO.Data;
using BaseProject.DAO.IService;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.DAO.Hangfire
{
	public class HangfireJobs
	{

		private readonly ApplicationDbContext _context = new();
		private readonly ILogger<HangfireJobs> _logger;
		private readonly IServiceEmail _serviceEmail;

		public HangfireJobs
		(
			ILogger<HangfireJobs> logger,
			IServiceEmail serviceEmail
		)
		{
			_logger = logger;
			_serviceEmail = serviceEmail;
		}

		public HangfireJobs() { }

		public void LimparLogs()
		{
			int daysAgo = 30; //Defina a partir de quantos dias atrás os logs serão limpos 

			_context.Database.ExecuteSqlRaw("LimparLogs {0}", daysAgo);
		}

	}
}
