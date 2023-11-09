using BaseProject.DAO.IRepository;
using BaseProject.DAO.IService;
using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Filters;
using BaseProject.DAO.Models.Others;
using BaseProject.DAO.Models.Views;
using System.Linq.Expressions;

namespace BaseProject.DAO.Service
{
	public class ServiceLogAcessoUsuario : IServiceLogAcessoUsuario
    {
        private readonly IRepositoryLogAcessoUsuario _repositoryLogAcessoUsuario;

        public ServiceLogAcessoUsuario(IRepositoryLogAcessoUsuario repositoryLogAcessoUsuario)
        {
            _repositoryLogAcessoUsuario = repositoryLogAcessoUsuario;
        }

        public LogAcessoUsuario ObterPorId(int id, string includeProperties = "")
        {
            return _repositoryLogAcessoUsuario.FirstOrDefault(x => x.Id == id, includeProperties);
        }

        public LogAcessoUsuario[] ObterTodos(string includeProperties = "", bool noTracking = true)
        {
            return _repositoryLogAcessoUsuario.Get(includeProperties: includeProperties, noTracking: noTracking);
        }

        public bool Adicionar(LogAcessoUsuario entity)
        {
            return _repositoryLogAcessoUsuario.Insert(entity);
        }

        public bool Editar(LogAcessoUsuario entity)
        {
            return _repositoryLogAcessoUsuario.Update(entity);
        }

        public bool Deletar(int id)
        {
            return _repositoryLogAcessoUsuario.Delete(id);
        }

        public DTResult<LogAcessoUsuarioVM> Listar(DTParam param)
        {            
            var query = _repositoryLogAcessoUsuario.GetContext().Set<LogAcessoUsuario>().AsQueryable();

            //Adicione as colunas de texto onde a pesquisa geral será aplicada
            var search = param.SearchValue();
            if (!string.IsNullOrEmpty(search)) query = query.Where(x =>
                x.EnderecoIP.Contains(search) ||
                x.Dispositivo.Contains(search) ||
                x.Plataforma.Contains(search) ||
                x.Navegador.Contains(search)
            );

            //Adicione as colunas ordenáveis e o seu respectivo nome do datatables (É necessário pelo menos uma como padrão)
            var keyGen = new KeySelectorGenerator<LogAcessoUsuario>(param.SortedColumnName());
            keyGen.AddKeySelector(x => x.Data, "Data");
            query = keyGen.Sort(query, param.IsAscendingSort());

            //Adicione os filtros avançados
            var filters = param.Filters;
            if (filters is not null)
            {
                                
            }

            //Adicione as tabelas relacionadas caso precise
            var includeProperties = "IdUsuarioNavigation";

            var itens = _repositoryLogAcessoUsuario.Filter(query, param.InitialPosition(), param.ItensPerPage(), out int total, includeProperties);

            return new DTResult<LogAcessoUsuarioVM>
            {
                Itens = itens.Select(x => new LogAcessoUsuarioVM(x)).ToArray(),
                Total = total
            };
        }

        public DTResult<LogAcessoUsuarioVM> Listar(DTParam param, int idUsuario)
        {
            var query = _repositoryLogAcessoUsuario.GetContext().Set<LogAcessoUsuario>().AsQueryable();

            query = query.Where(x => x.IdUsuario == idUsuario);

            //Adicione as colunas de texto onde a pesquisa geral será aplicada
            var search = param.SearchValue();
            if (!string.IsNullOrEmpty(search)) query = query.Where(x =>
                x.EnderecoIP.Contains(search) ||
                x.Dispositivo.Contains(search) ||
                x.Plataforma.Contains(search) ||
                x.Navegador.Contains(search)
            );

            //Adicione as colunas ordenáveis e o seu respectivo nome do datatables (É necessário pelo menos uma como padrão)
            var keyGen = new KeySelectorGenerator<LogAcessoUsuario>(param.SortedColumnName());
            keyGen.AddKeySelector(x => x.Data, "Data");
            query = keyGen.Sort(query, param.IsAscendingSort());

            //Adicione os filtros avançados
            var filters = param.Filters;
            if (filters is not null)
            {

            }

            //Adicione as tabelas relacionadas caso precise
            var includeProperties = "IdUsuarioNavigation";

            var itens = _repositoryLogAcessoUsuario.Filter(query, param.InitialPosition(), param.ItensPerPage(), out int total, includeProperties);

            return new DTResult<LogAcessoUsuarioVM>
            {
                Itens = itens.Select(x => new LogAcessoUsuarioVM(x)).ToArray(),
                Total = total
            };
        }

    }
}
