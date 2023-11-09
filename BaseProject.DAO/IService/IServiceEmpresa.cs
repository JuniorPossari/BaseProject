using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Filters;
using BaseProject.DAO.Models.Others;
using BaseProject.DAO.Models.Views;

namespace BaseProject.DAO.IService
{
	public interface IServiceEmpresa : IService<Empresa>
    {
        bool Existe(string cnpj);
        DTResult<EmpresaVM> Listar(DTParam<EmpresaFM> param);
        S2Result ListarSelect2(S2Param param);
    }
}
