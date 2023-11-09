using BaseProject.DAO.IRepository;
using BaseProject.DAO.IService;
using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Filters;
using BaseProject.DAO.Models.Others;
using BaseProject.DAO.Models.Views;
using System.Text;

namespace BaseProject.DAO.Service
{
	public class ServiceUsuario : IServiceUsuario
    {
        private readonly IRepositoryUsuario _repositoryUsuario;

        public ServiceUsuario(IRepositoryUsuario repositoryUsuario)
        {
            _repositoryUsuario = repositoryUsuario;
        }

        public Usuario ObterPorId(int id, string includeProperties = "")
        {
            return _repositoryUsuario.FirstOrDefault(x => x.Id == id, includeProperties);
        }

        public Usuario ObterPorIdAspNetUser(string idAspNetUser, string includeProperties = "")
        {
            return _repositoryUsuario.FirstOrDefault(x => x.IdAspNetUser == idAspNetUser, includeProperties);
        }

        public Usuario[] ObterPorIdEmpresa(int idEmpresa, string includeProperties = "")
        {
            return _repositoryUsuario.Get(x => x.IdEmpresa == idEmpresa, includeProperties: includeProperties);
        }

        public Usuario[] ObterTodos(string includeProperties = "", bool noTracking = true)
        {
            return _repositoryUsuario.Get(includeProperties: includeProperties, noTracking: noTracking);
        }

        public bool Adicionar(Usuario entity)
        {
            return _repositoryUsuario.Insert(entity);
        }

        public bool Editar(Usuario entity)
        {
            return _repositoryUsuario.Update(entity);
        }

        public bool Deletar(int id)
        {
            return _repositoryUsuario.Delete(id);
        }

        public bool EditarSenha(int id, string senha)
        {
            var usuario = _repositoryUsuario.Get(x => x.Id == id).FirstOrDefault();

            usuario.Senha = Convert.ToBase64String(Encoding.ASCII.GetBytes(senha));

            return _repositoryUsuario.Update(usuario);
        }

        public bool Existe(string email, string cpf)
        {
            return _repositoryUsuario.Exists(x => x.Email == email || x.CPF == cpf);
        }

        public bool ExisteEmail(string email)
        {
            return _repositoryUsuario.Exists(x => x.Email == email);
        }

        public bool ExisteCPF(string cpf)
        {
            return _repositoryUsuario.Exists(x => x.CPF == cpf);
        }

        public DTResult<UsuarioVM> Listar(DTParam<UsuarioFM> param)
        {
            var query = _repositoryUsuario.GetContext().Set<Usuario>().AsQueryable();

            //Adicione as colunas de texto onde a pesquisa geral será aplicada
            var search = param.SearchValue();
            if (!string.IsNullOrEmpty(search)) query = query.Where(x =>
                x.Nome.Contains(search) ||
                x.Email.Contains(search) ||
                x.CPF.Contains(search)
            );

            //Adicione as colunas ordenáveis e o seu respectivo nome do datatables (É necessário pelo menos uma como padrão)
            var keyGen = new KeySelectorGenerator<Usuario>(param.SortedColumnName());
            keyGen.AddKeySelector(x => x.Id, "Id");
            keyGen.AddKeySelector(x => x.Nome, "Nome");
            keyGen.AddKeySelector(x => x.Email, "Email");
            keyGen.AddKeySelector(x => x.CPF, "CPF");
            keyGen.AddKeySelector(x => x.IdEmpresaNavigation.NomeFantasia, "NomeEmpresa");            
            keyGen.AddKeySelector(x => x.DataCadastro, "DataCadastro");
            query = keyGen.Sort(query, param.IsAscendingSort());

            //Adicione os filtros avançados
            var filters = param.Filters;
            if (filters is not null)
            {
                if (filters.IdEmpresa.HasValue) query = query.Where(x => x.IdEmpresa == filters.IdEmpresa.Value);
                if (filters.DataCadastro.HasValue) query = query.Where(x => x.DataCadastro.Date == filters.DataCadastro.Value.Date);
                if (filters.Ativo.HasValue) query = query.Where(x => x.Ativo == filters.Ativo.Value);
            }

            //Adicione as tabelas relacionadas caso precise
            var includeProperties = "IdEmpresaNavigation";

            var itens = _repositoryUsuario.Filter(query, param.InitialPosition(), param.ItensPerPage(), out int total, includeProperties);

            return new DTResult<UsuarioVM>
            {
                Itens = itens.Select(x => new UsuarioVM(x)).ToArray(),
                Total = total
            };
        }

    }
}
