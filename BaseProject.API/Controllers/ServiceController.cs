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
using Newtonsoft.Json.Linq;
using BaseProject.DAO.Models.Views;

namespace BaseProject.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ServiceController : Controller
    {
        private readonly ILogger<ServiceController> _logger;
        private UserManager<AspNetUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServiceEmpresa _serviceEmpresa;
        private readonly IServiceEmail _serviceEmail;
        private readonly IConverter _converter;

        public ServiceController(
            ILogger<ServiceController> logger,
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

        [AllowAnonymous]
        [HttpGet("GerarUsuario")]
        public async Task<IActionResult> GerarUsuario()
        {
            //Preencha os dados e acesse https://localhost:44301/Service/GerarUsuario
            string email = "admin@pro4tech.com.br"; //Seu email
            string nome = "Admin"; //Seu nome
            string senha = "@Sp230198"; //Sua senha

            bool sucesso = true;

            try
            {
                sucesso = await GerarPermissao();

                if (!sucesso)
                {
                    return Json(new
                    {
                        Ok = false,
                        Title = "Erro",
                        Message = "Erro ao gerar a permissão de administrador do usuário!",
                    });
                }

                sucesso = GerarEmpresa();

                if (!sucesso)
                {
                    return Json(new
                    {
                        Ok = false,
                        Title = "Erro",
                        Message = "Erro ao gerar a empresa do usuário!",
                    });
                }

                var user = new AspNetUser
                {
                    UserName = email,
                    Email = email,
                    TwoFactorEnabled = false, //Ativa a verificação em 2 etapas para o usuário
                    Usuario = new Usuario
                    {
                        IdEmpresa = 1,
                        Nome = nome,
                        Email = email,
                        CPF = StringExtensions.RandomCPF(false),
                        Senha = Convert.ToBase64String(Encoding.ASCII.GetBytes(senha)),
                        DataCadastro = DateTime.Now.ToBrasiliaTime(),
                        Ativo = true,
                    }
                };

                var existeUsuario = _serviceUsuario.Existe(user.Usuario.Email, user.Usuario.CPF);

                if (!existeUsuario)
                {
                    var chkUser = await _userManager.CreateAsync(user, senha);

                    sucesso = chkUser.Succeeded;

                    if (sucesso)
                    {
                        var roles = new List<string>();

                        roles.Add("Administrador");
                        roles.Add("Gerente");

                        var chkRole = await _userManager.AddToRolesAsync(user, roles);
                        sucesso = chkRole.Succeeded;
                    }
                }

            }
            catch (Exception e)
            {
                sucesso = false;
            }

            return Json(new
            {
                Ok = sucesso,
                Title = sucesso ? "Sucesso" : "Erro",
                Message = sucesso ? "Sucesso ao gerar o usuário!" : "Erro ao gerar o usuário!",
            });
        }

        private async Task<bool> GerarPermissao()
        {
            bool sucesso = true;

            try
            {
                var role = new IdentityRole
                {
                    Name = "Administrador"
                };

                bool roleExists = await _roleManager.RoleExistsAsync(role.Name);

                if (!roleExists)
                {
                    var chkRole = await _roleManager.CreateAsync(role);

                    sucesso = chkRole.Succeeded;
                }

                role = new IdentityRole
                {
                    Name = "Gerente"
                };

                roleExists = await _roleManager.RoleExistsAsync(role.Name);

                if (!roleExists)
                {
                    var chkRole = await _roleManager.CreateAsync(role);

                    sucesso = chkRole.Succeeded;
                }
            }
            catch (Exception e)
            {
                sucesso = false;
            }

            return sucesso;
        }

        private bool GerarEmpresa()
        {
            bool sucesso = true;

            try
            {
                var empresa = new Empresa
                {
                    RazaoSocial = "PRO4TECH TECNOLOGIA E CONSULTORIA LTDA",
                    NomeFantasia = "Pro4Tech",
                    CNPJ = "22869671000126",
                    DataCadastro = DateTime.Now.ToBrasiliaTime(),
                    Ativa = true
                };

                bool empresaExists = _serviceEmpresa.Existe(empresa.CNPJ);

                if (!empresaExists)
                {
                    sucesso = _serviceEmpresa.Adicionar(empresa);
                }
            }
            catch (Exception e)
            {
                sucesso = false;
            }

            return sucesso;
        }

        [HttpPost("FileCallback")]
        public IActionResult FileCallback()
        {
            return Ok();
        }

        [HttpPost("DefineSession")]
        public IActionResult DefineSession([FromBody] SessionObject sessionContent)
        {
            HttpContext.Session.SetString("requestObject", sessionContent.jsonData.ToString());
            return Json(this.CreateResponseObject(true));
        }

    }
}