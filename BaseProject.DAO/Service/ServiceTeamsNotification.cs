using BaseProject.DAO.IService;
using System.Text;

namespace BaseProject.DAO.Service
{

	public class ServiceTeamsNotification : IServiceTeamsNotification
    {

        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ServiceTeamsNotification> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly string _url = "Coloque a url aqui";

        public ServiceTeamsNotification() { }

        public ServiceTeamsNotification(
            IHttpClientFactory clientFactory,
            ILogger<ServiceTeamsNotification> logger,
            IWebHostEnvironment env
        )
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _env = env;
        }

		public async Task<bool> SendNotification(string title, string message)
		{
			try
			{
				var client = _clientFactory.CreateClient();

				var methodName = (new System.Diagnostics.StackTrace()).GetFrame(6).GetMethod().Name;

				title = title + " - " + methodName + " - " + _env.EnvironmentName;

				var response = await client.PostAsync(_url, new StringContent("{'title': '" + title + "', 'textFormat': 'markdown', 'text': '" + message + "'}", Encoding.UTF8, "application/json"));

				if (response.IsSuccessStatusCode) return true;

				_logger.LogError(new Exception("Falha na requisição ao tentar enviar notificação ao Teams!"), $"Título: {title}; Mensagem {message};");
				return false;

			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Falha ao enviar notificação ao Teams! Título: {title}; Mensagem {message};");
				return false;
			}
		}

		public async Task<bool> SendNotificationAsync(string title, string message)
		{
			try
			{
				using var client = new HttpClient();

				var response = await client.PostAsync(_url, new StringContent("{'title': '" + title + "', 'textFormat': 'markdown', 'text': '" + message + "'}", Encoding.UTF8, "application/json"));

				if (response.IsSuccessStatusCode) return true;

				return false;

			}
			catch (Exception e)
			{
				return false;
			}
		}

	}
}