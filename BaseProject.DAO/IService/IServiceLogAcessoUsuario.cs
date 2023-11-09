using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Filters;
using BaseProject.DAO.Models.Others;
using BaseProject.DAO.Models.Views;

namespace BaseProject.DAO.IService
{
	public interface IServiceLogAcessoUsuario : IService<LogAcessoUsuario>
    {
        DTResult<LogAcessoUsuarioVM> Listar(DTParam param);
        DTResult<LogAcessoUsuarioVM> Listar(DTParam param, int idUsuario);
    }
}
