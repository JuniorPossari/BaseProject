using BaseProject.DAO.IService;
using BaseProject.DAO.Models.Others;
using BaseProject.Util;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BaseProject.DAO.Service
{

	public class ServiceEmail : IServiceEmail
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly string _key;
        private readonly string _address;
        private readonly string _name;

        public ServiceEmail(
            IConfiguration config,
            IWebHostEnvironment webHostEnvironment)
        {            
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _key = _config.GetProperty<string>("SendGrid", "Key");
            _address = _config.GetProperty<string>("SendGrid", "Address");
            _name = _config.GetProperty<string>("SendGrid", "Name");
        }

        public async Task<bool> SendEmail(EmailOptions options)
        {

            var client = new SendGridClient(_key);

            var from = new EmailAddress(_address, _name);

            var to = new EmailAddress(options.ToEmail);

            string subject = options.Subject;

            string htmlContent = UpdatePlaceHolders(GetEmailBody(options.Template), options.PlaceHolders);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

            var response = await client.SendEmailAsync(msg);

            bool sucesso = response.IsSuccessStatusCode;

            return sucesso;
        }

        private string GetEmailBody(string template)
        {
            string rootPath = _webHostEnvironment.WebRootPath;

            string templatePath = @"\Templates\{0}.html";

            var body = File.ReadAllText(string.Format(rootPath + templatePath, template));

            return body;
        }

        private string UpdatePlaceHolders(string text, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePairs != null)
            {
                foreach (var placeholder in keyValuePairs)
                {
                    if (text.Contains(placeholder.Key))
                    {
                        text = text.Replace(placeholder.Key, placeholder.Value);
                    }
                }
            }

            return text;
        }

    }
}
