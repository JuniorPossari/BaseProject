using Microsoft.AspNetCore.Mvc;
using BaseProject.DAO.IService;
using Microsoft.AspNetCore.Authorization;

namespace BaseProject.API.Areas.API.Controllers
{
    [Authorize]
    [Area("API")]
    [Route("[area]/[controller]/V1")]
    public class LocalidadeController : Controller
    {
        private readonly ILogger<LocalidadeController> _logger;
        private readonly IServiceLocalidade _serviceLocalidade;

        public LocalidadeController
        (
            ILogger<LocalidadeController> logger,
            IServiceLocalidade serviceLocalidade
        )
        {
            _logger = logger;
            _serviceLocalidade = serviceLocalidade;
        }

        [HttpGet("Paises")]
        public async Task<IActionResult> Paises()
        {
            return Json(await _serviceLocalidade.ObterPaises());
        }

        [HttpGet("Paises/{codPais}/Estados")]
        public async Task<IActionResult> Estados([FromRoute] string codPais)
        {
            return Json(await _serviceLocalidade.ObterEstados(codPais));
        }

        [HttpGet("Paises/{codPais}/Estados/{codEstado}/Cidades")]
        public async Task<IActionResult> Cidades([FromRoute] string codPais, [FromRoute] string codEstado)
        {
            return Json(await _serviceLocalidade.ObterCidades(codPais, codEstado));
        }

    }
}