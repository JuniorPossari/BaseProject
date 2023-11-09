using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BaseProject.DAO.IService;
using BaseProject.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using BaseProject.DAO.Models.Filters;
using BaseProject.DAO.Models.Views;
using BaseProject.DAO.Models.Others;
using BaseProject.Util;
using System.Text;

namespace BaseProject.API.Areas.Gerenciamento.Controllers
{
    [Authorize(Roles = "Gerente,Administrador")]
    [Area("Gerenciamento")]
    [Route("[area]/[controller]")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private UserManager<AspNetUser> _userManager;
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServiceEmail _serviceEmail;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsuarioController(
            ILogger<UsuarioController> logger,
            UserManager<AspNetUser> userManager,
            IServiceUsuario serviceUsuario,
            IServiceEmail serviceEmail,
            IConfiguration config,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _logger = logger;
            _userManager = userManager;
            _serviceUsuario = serviceUsuario;
            _serviceEmail = serviceEmail;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Listar")]
        public IActionResult Listar([FromBody] DTParam<UsuarioFM> param)
        {
            var result = _serviceUsuario.Listar(param);

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

        [HttpGet("GetTeste")]
        public JsonResult GetTeste()
        {
            var teste = this.CreateObjectFromSession<UsuarioVM>();
            var variable = this.CreateVariableFromSession<Int32>("id");
            return Json(this.CreateResponseObject(true, successMessage: "Sucesso ao enviar coisas"));
        }

        [HttpPost("PostTeste")]
        public JsonResult PostTeste([FromQuery] int idUsuario, [FromBody] UsuarioVM usuario)
        {
            var teste = this.CreateObjectFromSession<UsuarioVM>();
            var variable = this.CreateVariableFromSession<Int32>("id");
            var session = HttpContext.Session.GetString("requestObject");
            return Json(this.CreateResponseObject(true, successMessage: "Sucesso com o post!"));
        }

        [HttpPost("Adicionar")]
        public async Task<IActionResult> Adicionar([FromBody] Usuario model)
        {
            var existeEmail = _serviceUsuario.ExisteEmail(model.Email);

            if (existeEmail) return Json(this.CreateResponseObject(existeEmail, errorMessage: "Esse email já está vinculado a uma conta!"));
            
            var existeCPF = _serviceUsuario.ExisteCPF(model.CPF);

            if (existeCPF) return Json(this.CreateResponseObject(existeCPF, errorMessage: "Esse CPF já está vinculado a uma conta!"));

            var senha = StringExtensions.RandomPassword(14);

            var user = new AspNetUser
            {
                UserName = model.Email,
                Email = model.Email,
                TwoFactorEnabled = false, //Ativa a verificação em 2 etapas para o usuário
                Usuario = new Usuario
                {   
                    IdEmpresa = model.IdEmpresa,
                    Nome = model.Nome,
                    Email = model.Email,
                    CPF = model.CPF,
                    Senha = Convert.ToBase64String(Encoding.ASCII.GetBytes(senha)),
                    DataCadastro = DateTime.Now.ToBrasiliaTime(),
                    Ativo = model.Ativo,
                }
            };

            var chkUser = await _userManager.CreateAsync(user, senha);

            bool sucesso = chkUser.Succeeded;

            if (sucesso)
            {
                var emailOptions = new EmailOptions
                {
                    Subject = "Dados de acesso a sua conta!",
                    ToEmail = user.Email,
                    Template = EmailTemplate.NovoUsuario,
                    PlaceHolders = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("{{NOME}}", user.Usuario.Nome),
                        new KeyValuePair<string, string>("{{LOGIN}}", user.UserName),
                        new KeyValuePair<string, string>("{{SENHA}}", senha),
                        new KeyValuePair<string, string>("{{URL}}", _config.GetProperty<string>("AppUrl", _webHostEnvironment.EnvironmentName))
                    }
                };

                sucesso = await _serviceEmail.SendEmail(emailOptions);

                if(!sucesso) return Json(this.CreateResponseObject(false, errorMessage: "Sucesso ao adicionar o usuário, mas houve um erro ao enviar o email com os dados de acesso a sua conta. Por favor, contate um administrador!"));

            }

            return Json(this.CreateResponseObject(sucesso, "Sucesso ao adicionar o usuário!", "Erro ao adicionar o usuário!"));

        }

        [HttpGet("Editar/{id}")]
        public IActionResult Editar([FromRoute] int id)
        {
            var model = _serviceUsuario.ObterPorId(id, "IdAspNetUserNavigation,IdEmpresaNavigation");

            if (model == null) return NotFound();

            return View(new UsuarioVM(model));
        }

        [HttpPost("Editar")]
        public async Task<IActionResult> Editar([FromBody] Usuario model)
        {
            var user = _userManager.ObterPorIdUsuario(model.Id, "Usuario");

            if (user.Usuario.Email != model.Email)
            {
                var existeEmail = _serviceUsuario.ExisteEmail(model.Email);

                if (existeEmail) return Json(this.CreateResponseObject(false, errorMessage: "Esse email já está vinculado a uma conta!"));                    
                
            }

            if (user.Usuario.CPF != model.CPF)
            {
                var existeCPF = _serviceUsuario.ExisteCPF(model.CPF);

                if (existeCPF) return Json(this.CreateResponseObject(false, errorMessage: "Esse CPF já está vinculado a uma conta!"));                
            }

            user.UserName = model.Email;
            user.Email = model.Email;
            user.Usuario.IdEmpresa = model.IdEmpresa;
            user.Usuario.Nome = model.Nome;
            user.Usuario.Email = model.Email;
            user.Usuario.CPF = model.CPF;
            user.Usuario.Ativo = model.Ativo;

            var chkUser = await _userManager.UpdateAsync(user);

            bool sucesso = chkUser.Succeeded;

            return Json(this.CreateResponseObject(sucesso, "Sucesso ao editar o usuário!", "Erro ao editar o usuário!"));
        }

        [HttpGet("Visualizar/{id}")]
        public IActionResult Visualizar([FromRoute] int id)
        {
            var model = _serviceUsuario.ObterPorId(id, "IdAspNetUserNavigation,IdEmpresaNavigation");

            if (model == null) return NotFound();

            return View(new UsuarioVM(model));
        }

        [HttpPost("Deletar")]
        public async Task<IActionResult> Deletar([FromBody] int id)
        {
            var user = _userManager.ObterPorIdUsuario(id, "Usuario");

            var chkUser = await _userManager.DeleteAsync(user);

            bool sucesso = chkUser.Succeeded;

            return Json(new
            {
                Ok = sucesso,
                Title = sucesso ? "Sucesso" : "Erro",
                Message = sucesso ? "Sucesso ao deletar o usuário!" : "Erro ao deletar o usuário!",
            });
        }

        [HttpGet("ObterFoto")]
        public IActionResult ObterFoto([FromQuery] int idUsuario)
        {
            UsuarioFoto[] result = { _serviceUsuario.ObterPorId(idUsuario, "UsuarioFoto").UsuarioFoto };

            return Json(new
            {
                Result = result
            });
        }

    }
}