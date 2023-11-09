using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Filters;
using BaseProject.DAO.Models.Others;
using BaseProject.DAO.Models.Views;

namespace BaseProject.DAO.IService
{
	public interface IServiceUsuario : IService<Usuario>
    {
        Usuario ObterPorIdAspNetUser(string idAspNetUser, string includeProperties = "");
        Usuario[] ObterPorIdEmpresa(int idEmpresa, string includeProperties = "");
        bool EditarSenha(int id, string senha);
        bool Existe(string email, string cpf);
        bool ExisteEmail(string email);
        bool ExisteCPF(string cpf);
        DTResult<UsuarioVM> Listar(DTParam<UsuarioFM> param);
    }
}
