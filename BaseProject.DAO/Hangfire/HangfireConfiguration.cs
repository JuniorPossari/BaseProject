using Hangfire;
using Hangfire.Storage;

namespace BaseProject.DAO.Hangfire
{
	public class HangfireConfiguration
	{

		private readonly RecurringJobOptions _recurringJobOptions = new() { TimeZone = TimeZoneInfo.Local };

		public void AddRecurringJobs()
		{
			DeleteRecurringJobs(); //Deleta todos os jobs recorrentes para adicionar somentes os que foram configurados abaixo

			//Limpa os logs diariamente as 3 horas da madrugada
			RecurringJob.AddOrUpdate<HangfireJobs>("LimparLogs", x => x.LimparLogs(), Cron.Daily(3), _recurringJobOptions);
		}

		public void DeleteRecurringJobs()
		{
			using var connection = JobStorage.Current.GetConnection();

			foreach (var recurringJob in connection.GetRecurringJobs())
			{
				RecurringJob.RemoveIfExists(recurringJob.Id);
			}
		}

	}
}
