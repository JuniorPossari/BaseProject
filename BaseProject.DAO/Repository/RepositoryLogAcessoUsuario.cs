using BaseProject.DAO.Data;
using BaseProject.DAO.IRepository;
using BaseProject.DAO.Models;

namespace BaseProject.DAO.Repository
{
	public class RepositoryLogAcessoUsuario : Repository<LogAcessoUsuario, ApplicationDbContext>, IRepositoryLogAcessoUsuario
    {
        public RepositoryLogAcessoUsuario() : base() { }

        public RepositoryLogAcessoUsuario(ApplicationDbContext context) : base(context) { }

    }
}
