using BaseProject.DAO.Models.Others;
using BaseProject.DAO.Models.Views;
using BaseProject.Util;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.API.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("Error")]
        public ErrorResponse Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            return new ErrorResponse(exception);
        }

		[HttpGet("HttpError/{id:length(3,3)}/{responseType?}")]
		public IActionResult HttpError([FromRoute] int id, [FromRoute] string responseType = "")
		{
			var model = new ErrorVM();

			if (id == 500)
			{
				model.Mensagem = "Tente novamente mais tarde ou contate nosso suporte.";
				model.Titulo = "Ocorreu um erro!";
				model.ErroCode = id;
			}
			else if (id == 404)
			{
				model.Mensagem = "A página que está procurando não existe! <br />Em caso de dúvidas entre em contato com nosso suporte.";
				model.Titulo = "Ops! Página não encontrada.";
				model.ErroCode = id;
			}
			else if (id == 403)
			{
				model.Mensagem = "Você não tem permissão para acessar essa página.";
				model.Titulo = "Acesso Negado!";
				model.ErroCode = id;
			}
			else if (id == 401)
			{
				model.Mensagem = "Você não tem autorização para acessar essa página.";
				model.Titulo = "Não autorizado!";
				model.ErroCode = id;
			}

			if (responseType == "json")
				return Json(this.CreateResponseObject(false, errorMessage: "Erro ao processar a requisição, tente novamente mais tarde! Se o erro persistir, por favor, contate nosso suporte."));
			else
				return View("Error", model);
		}

		[HttpGet("TokenExpired")]
        public IActionResult TokenExpired()
        {
            var model = new ErrorVM();

            model.Mensagem = "Esse token já foi utilizado ou está expirado.";
            model.Titulo = "Token expirado!";
            model.ErroCode = 401;

            return View("Error", model);
        }

    }

}