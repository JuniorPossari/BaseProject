using BaseProject.DAO.IRepository;
using BaseProject.DAO.IService;
using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Filters;
using BaseProject.DAO.Models.Others;
using BaseProject.DAO.Models.Views;
using System.Linq.Expressions;

namespace BaseProject.DAO.Service
{
	public class ServiceEmpresa : IServiceEmpresa
    {
        private readonly IRepositoryEmpresa _repositoryEmpresa;

        public ServiceEmpresa(IRepositoryEmpresa repositoryEmpresa)
        {
            _repositoryEmpresa = repositoryEmpresa;
        }

        public Empresa ObterPorId(int id, string includeProperties = "")
        {
            return _repositoryEmpresa.FirstOrDefault(x => x.Id == id, includeProperties);
        }

        public Empresa[] ObterTodos(string includeProperties = "", bool noTracking = true)
        {
            return _repositoryEmpresa.Get(includeProperties: includeProperties, noTracking: noTracking);
        }

        public bool Adicionar(Empresa entity)
        {
            return _repositoryEmpresa.Insert(entity);
        }

        public bool Editar(Empresa entity)
        {
            return _repositoryEmpresa.Update(entity);
        }

        public bool Deletar(int id)
        {
            return _repositoryEmpresa.Delete(id);
        }

        public bool Existe(string cnpj)
        {
            return _repositoryEmpresa.Exists(x => x.CNPJ == cnpj);
        }

        public DTResult<EmpresaVM> Listar(DTParam<EmpresaFM> param)
        {            
            var query = _repositoryEmpresa.GetContext().Set<Empresa>().AsQueryable();

            //Adicione as colunas de texto onde a pesquisa geral será aplicada
            var search = param.SearchValue();
            if (!string.IsNullOrEmpty(search)) query = query.Where(x =>
                x.NomeFantasia.Contains(search) ||
                x.CNPJ.Contains(search)
            );

            //Adicione as colunas ordenáveis e o seu respectivo nome do datatables (É necessário pelo menos uma como padrão)
            var keyGen = new KeySelectorGenerator<Empresa>(param.SortedColumnName());
            keyGen.AddKeySelector(x => x.Id, "Id");
            keyGen.AddKeySelector(x => x.NomeFantasia, "NomeFantasia");
            keyGen.AddKeySelector(x => x.CNPJ, "CNPJ");
            keyGen.AddKeySelector(x => x.DataCadastro, "DataCadastro");
            query = keyGen.Sort(query, param.IsAscendingSort());

            //Adicione os filtros avançados
            var filters = param.Filters;
            if (filters is not null)
			{                
                if (filters.DataCadastro.HasValue) query = query.Where(x => x.DataCadastro.Date == filters.DataCadastro.Value.Date);
                if (filters.Ativa.HasValue) query = query.Where(x => x.Ativa == filters.Ativa.Value);
            }

            //Adicione as tabelas relacionadas caso precise
            var includeProperties = "";

            var itens = _repositoryEmpresa.Filter(query, param.InitialPosition(), param.ItensPerPage(), out int total, includeProperties);

            return new DTResult<EmpresaVM>
            {
                Itens = itens.Select(x => new EmpresaVM(x)).ToArray(),
                Total = total
            };
        }

        public S2Result ListarSelect2(S2Param param)
        {
            var query = _repositoryEmpresa.GetContext().Set<Empresa>().AsQueryable();                        
            var search = param.SearchValue();

            //Descomente a linha abaixo para aparecer resultados apenas após uma pesquisa
            //if (string.IsNullOrEmpty(search)) return new S2Result();

            //Adicione as colunas de texto onde a pesquisa geral será aplicada
            query = query.Where(x => x.NomeFantasia.Contains(search) || x.CNPJ.Contains(search));

            //Necessário ordenar por uma coluna
            query = query.OrderBy(x => x.NomeFantasia);

            var itens = _repositoryEmpresa.FilterSelect2(x => new S2Option 
            { 
                Id = x.Id, //Selecione qual será o Id e o Texto dos options no Select2
                Text = x.NomeFantasia
            }
            , query, param.InitialPosition(), param.ItensPerPage(), out int total);

            return new S2Result
            {
                Itens = itens,
                Total = total
            };
        }

    }
}
