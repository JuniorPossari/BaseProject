using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BaseProject.DAO.IService;
using BaseProject.DAO.Models;
using System.Diagnostics;
using BaseProject.DAO.Models.Others;
using ClosedXML.Excel;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using BaseProject.Util;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace BaseProject.API.Controllers
{
	[Authorize]
	public class DashboardController : Controller
	{
		private readonly ILogger<DashboardController> _logger;
		private UserManager<AspNetUser> _userManager;
		private RoleManager<IdentityRole> _roleManager;
		private readonly IServiceUsuario _serviceUsuario;
		private readonly IServiceEmpresa _serviceEmpresa;
		private readonly IServiceEmail _serviceEmail;
		private readonly IConverter _converter;		

		public DashboardController(
			ILogger<DashboardController> logger,
			UserManager<AspNetUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IServiceUsuario serviceUsuario,
			IServiceEmpresa serviceEmpresa,
			IServiceEmail serviceEmail,
			IConverter converter
		)
		{
			_logger = logger;
			_userManager = userManager;
			_roleManager = roleManager;
			_serviceUsuario = serviceUsuario;
			_serviceEmpresa = serviceEmpresa;
			_serviceEmail = serviceEmail;
			_converter = converter;
		}
				
		public IActionResult Index()
		{
			var user = _userManager.ObterPorUserName(User.Identity.Name);

			if(user == null)
			{
				foreach (var cookie in Request.Cookies)
				{
					if (cookie.Key == "acceptCookies") continue;
					Response.Cookies.Delete(cookie.Key);
				}

				return RedirectToAction("SignIn", "Account");
			}

			return View();
		}
		
	}
}