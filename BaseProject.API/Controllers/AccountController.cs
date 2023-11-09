using BaseProject.DAO.IService;
using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Others;
using BaseProject.DAO.Models.Views;
using BaseProject.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text;
using Wangkanai.Detection.Services;

namespace BaseProject.API.Controllers
{
	[Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private UserManager<AspNetUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private SignInManager<AspNetUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly IServiceEmail _serviceEmail;
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServiceToken _serviceToken;
        private readonly IDetectionService _detectionService;
        private readonly IServiceLogAcessoUsuario _serviceLogAcessoUsuario;
        private readonly IMemoryCache _memoryCache;

        public AccountController(
            ILogger<AccountController> logger,
            UserManager<AspNetUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AspNetUser> signInManager,
            IConfiguration config,
            IServiceEmail serviceEmail,
            IServiceUsuario serviceUsuario,
            IServiceToken serviceToken,
            IDetectionService detectionService,
            IServiceLogAcessoUsuario serviceLogAcessoUsuario,
            IMemoryCache memoryCache
        )
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _config = config;
            _serviceEmail = serviceEmail;
            _serviceUsuario = serviceUsuario;
            _serviceToken = serviceToken;
            _detectionService = detectionService;
            _serviceLogAcessoUsuario = serviceLogAcessoUsuario;
            _memoryCache = memoryCache;
        }

        //Cadastrar conta

        [HttpGet("SignUp")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpVM model)
        {
            if (!ModelState.IsValid) return Json(new { Ok = false, Title = "Erro", Message = "Dados inválidos!" });

            var existeUsuario = _serviceUsuario.Existe(model.Email, model.CPF);

            if (existeUsuario)
            {
                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = "Esse email ou CPF já está vinculado a uma conta!",
                });
            }

            var user = new AspNetUser
            {
                UserName = model.Email,
                Email = model.Email,
                TwoFactorEnabled = false, //Ativa a verificação em 2 etapas para o usuário
                Usuario = new Usuario
                {
                    IdEmpresa = 1, //Por padrão usa a primeira empresa cadastrada
                    Nome = model.Nome,
                    Email = model.Email,
                    CPF = model.CPF,
                    Senha = Convert.ToBase64String(Encoding.ASCII.GetBytes(model.Password)),
                    DataCadastro = DateTime.Now.ToBrasiliaTime(),
                    Ativo = true,
                }
            };

            var chkUser = await _userManager.CreateAsync(user, model.Password);

            bool sucesso = chkUser.Succeeded;

            return Json(new
            {
                Ok = sucesso,
                Title = sucesso ? "Sucesso" : "Erro",
                Message = sucesso ? "Sucesso ao criar a conta!" : "Erro ao criar a conta!",
            });
        }

        //Confirmar email

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var model = new EmailConfirmVM
            {
                Email = email
            };

            if (!string.IsNullOrEmpty(token))
            {
                var user = _userManager.ObterPorEmail(model.Email);

                bool verifyToken = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "EmailConfirmation", token);

                if (!verifyToken)
                {
                    return Redirect("/TokenExpired");
                }

                token = token.Replace(' ', '+');

                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    model.EmailVerified = true;
                }
            }

            return View(model);
        }

        private async Task<bool> SendEmailForEmailConfirmation(AspNetUser user)
        {
            try
            {
                await _userManager.UpdateSecurityStampAsync(user);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var emailConfirmLink = Url.Action("ConfirmEmail", "Account", new { email = user.Email, token = token }, Request.Scheme);

                var emailOptions = new EmailOptions
                {
                    Subject = "Confirmação de email!",
                    ToEmail = user.Email,                    
                    Template = EmailTemplate.ConfirmarEmail,
                    PlaceHolders = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("{{NOME}}", user.UserName),
                        new KeyValuePair<string, string>("{{URL}}", emailConfirmLink)
                    }
                };

                await _serviceEmail.SendEmail(emailOptions);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmVM model)
        {
            if (!ModelState.IsValid) return Json(new { Ok = false, Title = "Erro", Message = "Dados inválidos!" });

            var user = _userManager.ObterPorEmail(model.Email);

            if (user != null)
            {
                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    return Json(new
                    {
                        Ok = false,
                        Title = "Erro",
                        Message = "Esse email já foi confirmado!",
                    });
                }

                bool sucesso = await SendEmailForEmailConfirmation(user);

                if (!sucesso)
                {
                    return Json(new
                    {
                        Ok = false,
                        Title = "Erro",
                        Message = "Erro ao enviar o email de confirmação da sua conta!",
                    });
                }
            }

            return Json(new
            {
                Ok = true,
                Title = "Sucesso",
                Message = "Se você possui uma conta cadastrada nós enviamos um email com as instruções para confirma-la.<br />Por favor, verifique sua caixa de spam!",
            });
        }

        //Entrar na conta

        [HttpGet("SignIn")]
        public IActionResult SignIn()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInVM model)
        {
            if (!ModelState.IsValid) return Json(new { Ok = false, Title = "Erro", Message = "Dados inválidos!" });

            var user = _userManager.ObterPorEmail(model.Email, "Usuario.IdEmpresaNavigation");

            if(user == null)
			{
                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = "Email ou senha incorreto!",
                });
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                var now = DateTimeOffset.Now;
                var end = user.LockoutEnd.Value;
                var minutes = Math.Ceiling((end - now).TotalMinutes);
                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = $"Devido a várias tentativas de acesso, a sua conta foi bloqueada. Tente novamente em {minutes + (minutes == 1 ? " minuto" : " minutos")}!",
                });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);

            var usuario = user.Usuario;

            if (!result.Succeeded)
			{
                GerarLogAcessoUsuario(usuario.Id, false);

                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = "Email ou senha incorreto!",
                });
            }

            if (!usuario.Ativo || !usuario.IdEmpresaNavigation.Ativa)
            {
                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = "Sua conta foi desativada!",
                });
            }

            var twoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

			if (!twoFactorEnabled)
			{
                var roles = await _userManager.GetRolesAsync(user);
                var token = _serviceToken.GenerateToken(user, roles.ToList());

                Response.Cookies.Delete("redirectUrl");
                Response.Cookies.Append("access_token", token, _serviceToken.GenerateCookies());

                GerarLogAcessoUsuario(usuario.Id, true);

                return Json(new
                {
                    Ok = true,
                    Title = "Sucesso",
                    Message = "Sucesso na confirmação de login e senha!",
                });
            }

            var existsSecurityCode = _memoryCache.TryGetValue(user.UserName, out TwoStepsVM twoStepsVM);

            if (existsSecurityCode) _memoryCache.Remove(user.UserName);

            twoStepsVM = new TwoStepsVM
            {
                UserName = user.UserName,
                Code = StringExtensions.RandomNumber(6)
            };

            _memoryCache.Set(user.UserName, twoStepsVM, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3),
                SlidingExpiration = TimeSpan.FromMinutes(3)
            });

            var sucesso = await SendEmailSecutiryCode(user, twoStepsVM.Code);

            return Json(new
            {
                Ok = true,
                Title = "Sucesso",
                Message = "Sucesso na confirmação de login e senha!",
            });
        }

        //Verificação em duas etapas

        [HttpGet("TwoSteps/{username}")]
        public IActionResult TwoSteps([FromRoute] string username)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var user = _userManager.ObterPorUserName(username, "Usuario.IdEmpresaNavigation");

                if (user == null) return NotFound();

                var existsSecurityCode = _memoryCache.TryGetValue(user.UserName, out TwoStepsVM twoStepsVM);

                if (!existsSecurityCode) return NotFound();

                ViewBag.UserName = user.UserName;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost("TwoSteps")]
        public async Task<IActionResult> TwoSteps([FromBody] TwoStepsVM model)
        {
            if (!ModelState.IsValid) return Json(new { Ok = false, Title = "Erro", Message = "Dados inválidos!" });

            var user = _userManager.ObterPorUserName(model.UserName, "Usuario.IdEmpresaNavigation");

            if (user == null)
            {
                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = "Código expirado!",
                });
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                var now = DateTimeOffset.Now;
                var end = user.LockoutEnd.Value;
                var minutes = Math.Ceiling((end - now).TotalMinutes);
                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = $"Devido a várias tentativas de acesso, a sua conta foi bloqueada. Tente novamente em {minutes + (minutes == 1 ? " minuto" : " minutos")}!",
                });
            }

            var existsSecurityCode = _memoryCache.TryGetValue(user.UserName, out TwoStepsVM twoStepsVM);

            if (!existsSecurityCode)
            {
                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = "Código expirado! Reenvie um novo código de segurança para tentar novamente!",
                });
            }

            if(model.UserName != twoStepsVM.UserName || model.Code != twoStepsVM.Code)
			{
                await _signInManager.PasswordSignInAsync(user, "", true, true); //Gera tentativa de acesso para bloqueio da conta

                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = "Código de segurança incorreto!",
                });
            }

            _memoryCache.Remove(user.UserName);

            var usuario = user.Usuario;

            var roles = await _userManager.GetRolesAsync(user);
            var token = _serviceToken.GenerateToken(user, roles.ToList());

            Response.Cookies.Delete("redirectUrl");
            Response.Cookies.Append("access_token", token, _serviceToken.GenerateCookies());

            GerarLogAcessoUsuario(usuario.Id, true);

            return Json(new
            {
                Ok = true,
                Title = "Sucesso",
                Message = "Sucesso na autenticação de dois fatores!",
            });
        }

        [HttpPost("ResendSecurityCode")]
        public async Task<IActionResult> ResendSecurityCode([FromBody] ResendSecurityCodeVM model)
        {
            if (!ModelState.IsValid) return Json(new { Ok = false, Title = "Erro", Message = "Dados inválidos!" });

            var user = _userManager.ObterPorUserName(model.UserName, "Usuario.IdEmpresaNavigation");

            if (user == null)
            {
                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = "Erro ao reenviar o código de segurança!",
                });
            }

            var existsSecurityCode = _memoryCache.TryGetValue(user.UserName, out TwoStepsVM twoStepsVM);

            if (existsSecurityCode) _memoryCache.Remove(user.UserName);

            twoStepsVM = new TwoStepsVM
            {
                UserName = user.UserName,
                Code = StringExtensions.RandomNumber(6)
            };

            _memoryCache.Set(user.UserName, twoStepsVM, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3),
                SlidingExpiration = TimeSpan.FromMinutes(3)
            });

            var sucesso = await SendEmailSecutiryCode(user, twoStepsVM.Code);

            return Json(new
            {
                Ok = sucesso,
                Title = sucesso ? "Sucesso" : "Erro",
                Message = sucesso ? "Sucesso ao reenviar o código de segurança!" : "Erro ao reenviar o código de segurança!",
            });
        }

        private async Task<bool> SendEmailSecutiryCode(AspNetUser user, string codigo)
        {
            try
            {
                var emailOptions = new EmailOptions
                {
                    Subject = "Código de segurança!",
                    ToEmail = user.Email,
                    Template = EmailTemplate.CodigoSeguranca,
                    PlaceHolders = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("{{NOME}}", user.Usuario.Nome),
                        new KeyValuePair<string, string>("{{CODIGO}}", codigo)
                    }
                };

                var sucesso = await _serviceEmail.SendEmail(emailOptions);

                return sucesso;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        //[HttpPost("SignIn")]
        //public async Task<IActionResult> SignIn([FromBody] SignInVM model)
        //{
        //    if (!ModelState.IsValid) return Json(new { Ok = false, Title = "Erro", Message = "Dados inválidos!" });

        //    var user = _userManager.ObterPorEmail(model.Email, "Usuario.IdEmpresaNavigation");

        //    if (user == null)
        //    {
        //        return Json(new
        //        {
        //            Ok = false,
        //            Title = "Erro",
        //            Message = "Email ou senha incorreto!",
        //        });
        //    }

        //    if (await _userManager.IsLockedOutAsync(user))
        //    {
        //        var now = DateTimeOffset.Now;
        //        var end = user.LockoutEnd.Value;
        //        var minutes = Math.Ceiling((end - now).TotalMinutes);
        //        return Json(new
        //        {
        //            Ok = false,
        //            Title = "Erro",
        //            Message = $"Devido a várias tentativas de acesso, a sua conta foi bloqueada. Tente novamente em {minutes + (minutes == 1 ? " minuto" : " minutos")}!",
        //        });
        //    }

        //    var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);

        //    var usuario = user.Usuario;

        //    if (!result.Succeeded)
        //    {
        //        GerarLogAcessoUsuario(usuario.Id, false);

        //        return Json(new
        //        {
        //            Ok = false,
        //            Title = "Erro",
        //            Message = "Email ou senha incorreto!",
        //        });
        //    }

        //    if (!usuario.Ativo || !usuario.IdEmpresaNavigation.Ativa)
        //    {
        //        return Json(new
        //        {
        //            Ok = false,
        //            Title = "Erro",
        //            Message = "Sua conta foi desativada!",
        //        });
        //    }

        //    var roles = await _userManager.GetRolesAsync(user);
        //    var token = _serviceToken.GenerateToken(user, roles.ToList());

        //    Response.Cookies.Delete("redirectUrl");
        //    Response.Cookies.Append("access_token", token, _serviceToken.GenerateCookies());

        //    GerarLogAcessoUsuario(usuario.Id, true);

        //    return Json(new
        //    {
        //        Ok = true,
        //        Title = "Sucesso",
        //        Message = "Sucesso na confirmação de login e senha!",
        //    });
        //}

        //Gerar log de acesso
        private bool GerarLogAcessoUsuario(int idUsuario, bool status)
		{
            var enderecoIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            if (string.IsNullOrEmpty(enderecoIP) || enderecoIP == "::1") enderecoIP = "127.0.0.1";

            var logAcessoUsuario = new LogAcessoUsuario
            {
                IdUsuario = idUsuario,
                EnderecoIP = enderecoIP,
                Dispositivo = _detectionService.Device.Type.ToString(),
                Plataforma = _detectionService.Platform.Name + " " + _detectionService.Platform.Version.ToString() + " " + _detectionService.Platform.Processor.ToString(),
                Navegador = _detectionService.Browser.Name + " " + _detectionService.Browser.Version.ToString(),
                Status = status,
                Data = DateTime.Now.ToBrasiliaTime(),
            };

            return _serviceLogAcessoUsuario.Adicionar(logAcessoUsuario);
        }        

        //Sair da conta

        [Authorize]
        [HttpGet("SignOut")]
        public IActionResult SignOut()
        {
            foreach (var cookie in Request.Cookies)
            {
                if (cookie.Key == "acceptCookies") continue;
                Response.Cookies.Delete(cookie.Key);
            }

            return RedirectToAction("SignIn", "Account");
        }

        //Redefinir a senha

        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordVM model)
        {
            if (!ModelState.IsValid) return Json(new { Ok = false, Title = "Erro", Message = "Dados inválidos!" });

            var user = _userManager.ObterPorEmail(model.Email, "Usuario");

            if (user != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                var emailOptions = new EmailOptions
                {
                    Subject = "Redefinição de senha!",
                    ToEmail = user.Email,
                    Template = EmailTemplate.RedefinirSenha,
                    PlaceHolders = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("{{NOME}}", user.Usuario.Nome),
                        new KeyValuePair<string, string>("{{URL}}", passwordResetLink)
                    }
                };

                bool sucesso = await _serviceEmail.SendEmail(emailOptions);

				if (!sucesso)
				{
                    return Json(new
                    {
                        Ok = false,
                        Title = "Erro",
                        Message = "Erro ao enviar o email para redefinir sua senha!",
                    });
                }
            }

            return Json(new
            {
                Ok = true,
                Title = "Sucesso",
                Message = "Se você possui uma conta cadastrada nós enviamos um email com as instruções para redefinir sua senha.<br />Por favor, verifique sua caixa de spam!",
            });

        }

        [HttpGet("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            bool result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", token);

            if (result)
            {
                return View();
            }

            return Redirect("/TokenExpired");

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return Json(new { Ok = false, Title = "Erro", Message = "Dados inválidos!" });

            var user = _userManager.ObterPorEmail(model.Email, "Usuario");

            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                {
                    _serviceUsuario.EditarSenha(user.Usuario.Id, model.Password);
                    await _userManager.RemoveAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword");

                    return Json(new
                    {
                        Ok = true,
                        Title = "Sucesso",
                        Message = "Sucesso ao redefinir sua senha!",
                    });
                }
            }

            return Json(new
            {
                Ok = false,
                Title = "Erro",
                Message = "Erro ao redefinir sua senha!",
            });
        }

        //Mudar senha

        [Authorize]
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVM model)
        {
            if (!ModelState.IsValid) return Json(new { Ok = false, Title = "Erro", Message = "Dados inválidos!" });

            var user = _userManager.ObterPorUserName(User.Identity.Name, "Usuario");

            bool sucesso = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);

			if (!sucesso)
			{
                return Json(new
                {
                    Ok = false,
                    Title = "Erro",
                    Message = "Senha atual incorreta!",
                });
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);            

            sucesso = result.Succeeded;

            if(sucesso) _serviceUsuario.EditarSenha(user.Usuario.Id, model.NewPassword);

            return Json(new
            {
                Ok = sucesso,
                Title = sucesso ? "Sucesso" : "Erro",
                Message = sucesso ? "Sucesso ao editar sua senha!" : "Erro ao editar sua senha!",
            });
        }

        [Authorize]
        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            var user = _userManager.ObterPorUserName(User.Identity.Name, "Usuario.IdEmpresaNavigation,Usuario.UsuarioFoto");

            if (user == null) return NotFound();

            return View(new UsuarioVM(user.Usuario));
        }

        [Authorize]
        [HttpGet("Profile/{id}")]
        public IActionResult Profile([FromRoute] string id)
        {
            var user = _userManager.ObterPorId(id, "Usuario.IdEmpresaNavigation,Usuario.UsuarioFoto");

            if(user == null) return NotFound();

            return View(new UsuarioVM(user.Usuario));
        }

        [Authorize]
        [HttpPost("ListarLogAcessoUsuario")]
        public IActionResult ListarLogAcessoUsuario([FromBody] DTParam param)
        {
            var idUsuario = Int32.Parse(User.FindFirstValue("IdUsuario"));

            var result = _serviceLogAcessoUsuario.Listar(param, idUsuario);

            return Json(new
            {
                draw = param.Draw,
                recordsFiltered = result.Total,
                recordsTotal = result.Total,
                data = result.Itens
            });
        }

        [Authorize]
        [HttpGet("GetAvatar")]
        public IActionResult GetAvatar()
        {
            return Json(new
            {
				_userManager.ObterPorUserName(User.Identity.Name, "Usuario.UsuarioFoto").Usuario.UsuarioFoto
            });
        }

        [Authorize]
        [HttpPost("ChangeAvatar")]
        public IActionResult ChangeAvatar([FromBody] UsuarioFoto model)
        {
            var usuario = _userManager.ObterPorUserName(User.Identity.Name, "Usuario.UsuarioFoto").Usuario;

            usuario.UsuarioFoto = model;

            bool sucesso = _serviceUsuario.Editar(usuario);

            return Json(new
            {
                Ok = sucesso,
                Title = sucesso ? "Sucesso" : "Erro",
                Message = sucesso ? "Sucesso ao editar sua foto!" : "Erro ao editar sua foto!",
            });
        }

		//[Authorize]
		//[HttpGet("TwoFactorAuthentication")]
		//public IActionResult TwoFactorAuthentication()
		//{
		//	var user = _userManager.ObterPorUserName(User.Identity.Name, "Usuario.IdEmpresaNavigation");

		//	if (user == null) return NotFound();

		//	return View(new UsuarioVM(user.Usuario));
		//}

	}
}
