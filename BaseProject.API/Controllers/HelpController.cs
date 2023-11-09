using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BaseProject.DAO.IService;
using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Others;
using ClosedXML.Excel;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using BaseProject.Util;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Security.Claims;

namespace BaseProject.API.Controllers
{
	[Authorize]
	[Route("[controller]")]
	public class HelpController : Controller
	{
		private readonly ILogger<HelpController> _logger;
		private UserManager<AspNetUser> _userManager;
		private RoleManager<IdentityRole> _roleManager;
		private readonly IServiceUsuario _serviceUsuario;
		private readonly IServiceEmpresa _serviceEmpresa;
		private readonly IServiceEmail _serviceEmail;
		private readonly IConverter _converter;	
		private readonly IConfiguration _config;
		private readonly IWebHostEnvironment _webHostEnvironment; 

		public HelpController(
			ILogger<HelpController> logger,
			UserManager<AspNetUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IServiceUsuario serviceUsuario,
			IServiceEmpresa serviceEmpresa,
			IServiceEmail serviceEmail,
			IConverter converter,
			IConfiguration config,
			IWebHostEnvironment webHostEnvironment
		)
		{
			_logger = logger;
			_userManager = userManager;
			_roleManager = roleManager;
			_serviceUsuario = serviceUsuario;
			_serviceEmpresa = serviceEmpresa;
			_serviceEmail = serviceEmail;
			_converter = converter;
			_config = config;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpGet("ExemploObterDadosDoUsuarioLogado")]
		public IActionResult ExemploObterDadosDoUsuarioLogado()
		{
			//Para testar acesse https://localhost:44301/Help/ExemploObterDadosDoUsuarioLogado
			//Só deve ser usado em métodos ou controllers com [Authorize] (Quando o usuário está logado)
			var idAspNetUser = User.FindFirstValue("IdAspNetUser");
			var idUsuario = Int32.Parse(User.FindFirstValue("IdUsuario"));
			var idEmpresa = Int32.Parse(User.FindFirstValue("IdEmpresa"));
			var roles = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
			
			var permissoes = "(";
			roles.ForEach(role => permissoes += role + ";");
			permissoes += ")";

			return Json(new
			{
				Ok = true,
				Title = "Sucesso",
				Message = $"IdAspNetUser: {idAspNetUser}; IdUsuario: {idUsuario}; IdEmpresa: {idEmpresa}; Permissões: {permissoes}",
			});
		}

		[HttpGet("ExemploEnviarEmail")]
		public async Task<IActionResult> ExemploEnviarEmail()
		{
			//Preencha os dados e acesse https://localhost:44301/Help/ExemploEnviarEmail
			var emailOptions = new EmailOptions
			{
				Subject = "Teste", // Assunto do email
				ToEmail = "exemplo@outlook.com", // Endereço de quem vai receber o email
				Template = EmailTemplate.NovoUsuario, // Vá na pasta wwwroot/Templates para criar um novo
				PlaceHolders = new List<KeyValuePair<string, string>>() // Veja no template o que é necessário substituir
				{
					new KeyValuePair<string, string>("{{NOME}}", "UserName"),
					new KeyValuePair<string, string>("{{LOGIN}}", "exemplo@outlook.com"),
					new KeyValuePair<string, string>("{{SENHA}}", "123456"),
					new KeyValuePair<string, string>("{{URL}}", _config.GetProperty<string>("AppUrl", _webHostEnvironment.EnvironmentName))
				}
			};

			bool sucesso = await _serviceEmail.SendEmail(emailOptions);

			return Json(new
			{
				Ok = sucesso,
				Title = sucesso ? "Sucesso" : "Erro",
				Message = sucesso ? "Sucesso ao enviar o email!" : "Erro ao enviar o email!",
			});
		}

		[HttpGet("ExemploDownloadExcel")]
		public IActionResult ExemploDownloadExcel()
		{
			//Para baixar o exemplo acesse https://localhost:44301/Help/ExemploDownloadExcel
			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("Usuários");

			worksheet.Row(1).Height = 25;
			worksheet.SheetView.FreezeRows(1);

			worksheet.Column(1).Width = 80;
			worksheet.Column(2).Width = 80;
			worksheet.Column(3).Width = 80;
			worksheet.Column(4).Width = 80;

			worksheet.Columns(1, 4).Style
				.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
				.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
				.Alignment.WrapText = true;

			worksheet.Cell(1, 1).SetValue("NOME").Style.Font.SetFontSize(14).Font.SetBold().Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromArgb(1, 35, 40, 71));
			worksheet.Cell(1, 2).SetValue("EMAIL").Style.Font.SetFontSize(14).Font.SetBold().Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromArgb(1, 35, 40, 71));
			worksheet.Cell(1, 3).SetValue("CPF").Style.Font.SetFontSize(14).Font.SetBold().Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromArgb(1, 35, 40, 71));
			worksheet.Cell(1, 4).SetValue("STATUS").Style.Font.SetFontSize(14).Font.SetBold().Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromArgb(1, 35, 40, 71));

			var linha = 1;

			var usuarios = _serviceUsuario.ObterTodos();

			foreach (var usuario in usuarios)
			{

				linha++;

				var nome = usuario.Nome;
				var email = usuario.Email;
				var cpf = usuario.CPF;
				var ativo = usuario.Ativo ? "Ativo" : "Inativo";

				worksheet.Cell(linha, 1).SetValue(nome);
				worksheet.Cell(linha, 2).SetValue(email);
				worksheet.Cell(linha, 3).SetValue(cpf);
				worksheet.Cell(linha, 4).SetValue(ativo);

			}

			using var ms = new MemoryStream();
			
			workbook.SaveAs(ms);
			workbook.Dispose();

			Response.Cookies.Append("fileDownload", "true");

			string nomeArquivo = $"EXEMPLO_EXCEL_USUÁRIOS_{DateTime.Now.ToBrasiliaTime():yyyy-MM-dd_HH-mm-ss}.xlsx";
			string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			byte[] bytes = ms.ToArray();

			var file = File(bytes, contentType, nomeArquivo);

			return file;
		}

		[HttpGet("ExemploDownloadPDF")]
		public async Task<IActionResult> ExemploDownloadPDF()
		{
			//Para baixar o exemplo acesse https://localhost:44301/Help/ExemploDownloadPDF
			var usuarios = _serviceUsuario.ObterTodos();

			string view = await this.RenderViewAsync("ExemploPDFUsuarios", usuarios);

			var pdf = new HtmlToPdfDocument()
			{
				GlobalSettings = {
					ColorMode = ColorMode.Color,
					Orientation = Orientation.Landscape,
					PaperSize = PaperKind.A4Plus,
					Margins =
					{
						Left = 0,
						Right = 0,
						Bottom = 0,
						Top = 20
					}
				},
				Objects = {
					new ObjectSettings() {
						PagesCount = true,
						HtmlContent = view,
						WebSettings = { DefaultEncoding = "utf-8" },
					},
				}
			};

			Response.Cookies.Append("fileDownload", "true");

			string nomeArquivo = $"EXEMPLO_PDF_USUÁRIOS_{DateTime.Now.ToBrasiliaTime():yyyy-MM-dd_HH-mm-ss}.pdf";
			string contentType = "application/pdf";
			byte[] bytes = _converter.Convert(pdf);

			var file = File(bytes, contentType, nomeArquivo);

			return file;
		}

	}
}