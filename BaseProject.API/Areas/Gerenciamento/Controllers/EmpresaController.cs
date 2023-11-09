using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BaseProject.DAO.IService;
using BaseProject.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using BaseProject.DAO.Models.Filters;
using BaseProject.DAO.Models.Views;
using BaseProject.DAO.Models.Others;
using BaseProject.Util;

namespace BaseProject.API.Areas.Gerenciamento.Controllers
{
	[Authorize(Roles = "Gerente,Administrador")]
	[Area("Gerenciamento")]
	[Route("[area]/[controller]")]
	public class EmpresaController : Controller
	{
		private readonly ILogger<EmpresaController> _logger;
		private UserManager<AspNetUser> _userManager;
		private readonly IServiceUsuario _serviceUsuario;
		private readonly IServiceEmpresa _serviceEmpresa;	

		public EmpresaController(
			ILogger<EmpresaController> logger,
			UserManager<AspNetUser> userManager,
			IServiceUsuario serviceUsuario,
			IServiceEmpresa serviceEmpresa
		)
		{
			_logger = logger;
			_userManager = userManager;
			_serviceUsuario = serviceUsuario;
			_serviceEmpresa = serviceEmpresa;
		}

		[HttpGet("Index")]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost("Listar")]
		public IActionResult Listar([FromBody] DTParam<EmpresaFM> param)
		{
			var result = _serviceEmpresa.Listar(param);

			return Json(new
			{
				draw = param.Draw,
				recordsFiltered = result.Total,
				recordsTotal = result.Total,
				data = result.Itens
			});
		}

		[HttpGet("Adicionar")]
		public IActionResult Adicionar()
		{
			return View();
		}

		[HttpPost("Adicionar")]
		public IActionResult Adicionar([FromBody] Empresa model)
		{
			model.DataCadastro = DateTime.Now.ToBrasiliaTime();

			bool sucesso = _serviceEmpresa.Adicionar(model);

			return Json(new
			{
				Ok = sucesso,
				Title = sucesso ? "Sucesso" : "Erro",
				Message = sucesso ? "Sucesso ao adicionar a empresa!" : "Erro ao adicionar a empresa!",
			});
		}

		[HttpGet("Editar/{id}")]
		public IActionResult Editar([FromRoute] int id)
		{
			var model = _serviceEmpresa.ObterPorId(id, "EmpresaLogo");

			if(model == null) return NotFound();

			return View(new EmpresaVM(model));
		}

		[HttpPost("Editar")]
		public IActionResult Editar([FromBody] Empresa model)
		{
			var empresa = _serviceEmpresa.ObterPorId(model.Id, "EmpresaLogo");

			empresa.RazaoSocial = model.RazaoSocial;
			empresa.NomeFantasia = model.NomeFantasia;
			empresa.CNPJ = model.CNPJ;
			empresa.Ativa = model.Ativa;
			empresa.EmpresaLogo = model.EmpresaLogo;

			bool sucesso = _serviceEmpresa.Editar(empresa);

			return Json(new
			{
				Ok = sucesso,
				Title = sucesso ? "Sucesso" : "Erro",
				Message = sucesso ? "Sucesso ao editar a empresa!" : "Erro ao editar a empresa!",
			});
		}

		[HttpGet("Visualizar/{id}")]
		public IActionResult Visualizar([FromRoute] int id)
		{
			var model = _serviceEmpresa.ObterPorId(id, "EmpresaLogo");

			if (model == null) return NotFound();

			return View(new EmpresaVM(model));
		}

		[HttpPost("Deletar")]
		public IActionResult Deletar([FromBody] int id)
		{
			var empresa = _serviceEmpresa.ObterPorId(id, "Usuario");

			if (empresa.Usuario.Any())
			{
				return Json(new
				{
					Ok = false,
					Title = "Erro",
					Message = "Essa empresa não pode ser deletada pois possui usuários!",
				});
			}

			bool sucesso = _serviceEmpresa.Deletar(id);

			return Json(new
			{
				Ok = sucesso,
				Title = sucesso ? "Sucesso" : "Erro",
				Message = sucesso ? "Sucesso ao deletar a empresa!" : "Erro ao deletar a empresa!",
			});
		}

		[HttpPost("ListarSelect2")]
		public IActionResult ListarSelect2([FromBody] S2Param param)
		{
			var result = _serviceEmpresa.ListarSelect2(param);

			return Json(new
			{
				results = result.Itens.Select(x => new { id = x.Id, text = x.Text }),
				pagination = new
				{
					more = param.More(result.Total)
				}
			});
		}

	}
}