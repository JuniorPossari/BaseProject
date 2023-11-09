using BaseProject.DAO.Models.Others;

namespace BaseProject.DAO.IService
{
	public interface IServiceLocalidade
    {
        Task<Pais[]> ObterPaises();
        Task<Estado[]> ObterEstados(string codPais);
        Task<Cidade[]> ObterCidades(string codPais, string codEstado);
    }
}
