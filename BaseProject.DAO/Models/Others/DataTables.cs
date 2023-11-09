using BaseProject.DAO.Models.Filters;
using System.Linq.Expressions;

namespace BaseProject.DAO.Models.Others
{
    public class DTParam
    {
        public int Draw { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public DTSearch Search { get; set; }

        public DTOrder[] Order { get; set; }

        public DTColumn[] Columns { get; set; }

        public DefaultFM Filters { get; set; }

        public int InitialPosition()
        {
            return Start;
        }

        public int ItensPerPage()
        {
            return Length;
        }

        public string SearchValue()
        {
            return Search.Value;
        }

        public bool IsAscendingSort()
        {
            return Order.FirstOrDefault() == null || Order.FirstOrDefault().Dir == "asc";
        }

        public string SortedColumnName()
        {
            if (Order.FirstOrDefault() == null) return "Nome";

            var columnName = Columns[Order[0].Column].Data;

            return string.IsNullOrEmpty(columnName) ? "Nome" : columnName;
        }

    }

    public class DTParam<TEntity>
    {
        public int Draw { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public DTSearch Search { get; set; }

        public DTOrder[] Order { get; set; }

        public DTColumn[] Columns { get; set; }

        public TEntity Filters { get; set; }

        public int InitialPosition()
		{
            return Start;
        }

        public int ItensPerPage()
        {
            return Length;
        }

        public string SearchValue()
        {
            return Search.Value;
        }

        public bool IsAscendingSort()
		{
            return Order.FirstOrDefault() == null || Order.FirstOrDefault().Dir == "asc";
		}

        public string SortedColumnName()
		{
            if (Order.FirstOrDefault() == null) return "Nome";

            var columnName = Columns[Order[0].Column].Data;

            return string.IsNullOrEmpty(columnName) ? "Nome" : columnName;
		}

    }

    public sealed class DTSearch
    {
        public string Value { get; set; }

        public bool Regex { get; set; }
    }

    public sealed class DTOrder
    {
        public int Column { get; set; }

        public string Dir { get; set; }
    }

    public sealed class DTColumn
    {
        public string Data { get; set; }

        public string Name { get; set; }

        public bool Orderable { get; set; }

        public bool Searchable { get; set; }

        public DTSearch Search { get; set; }
    }

    public class DTResult<T>
    {
        public T[] Itens { get; set; }
        public int Total { get; set; }
    }

    public class KeySelectorGenerator<TEntity>
    {
        public KeySelectorGenerator(string sortedColumnName)
        {
            StringKeySelectors = new Dictionary<string, Expression<Func<TEntity, string>>>();
            DoubleKeySelectors = new Dictionary<string, Expression<Func<TEntity, double>>>();
            IntKeySelectors = new Dictionary<string, Expression<Func<TEntity, int>>>();
            FloatKeySelectors = new Dictionary<string, Expression<Func<TEntity, float>>>();
            DatetimeKeySelector = new Dictionary<string, Expression<Func<TEntity, DateTime>>>();
            BoolKeySelectors = new Dictionary<string, Expression<Func<TEntity, bool>>>();
            SelectorNameAndType = new Dictionary<string, string>();
            SortedColumnName = sortedColumnName;
        }

        private Dictionary<string, Expression<Func<TEntity, string>>> StringKeySelectors { get; set; }
        private Dictionary<string, Expression<Func<TEntity, double>>> DoubleKeySelectors { get; set; }
        private Dictionary<string, Expression<Func<TEntity, int>>> IntKeySelectors { get; set; }
        private Dictionary<string, Expression<Func<TEntity, float>>> FloatKeySelectors { get; set; }
        private Dictionary<string, Expression<Func<TEntity, DateTime>>> DatetimeKeySelector { get; set; }
        private Dictionary<string, Expression<Func<TEntity, bool>>> BoolKeySelectors { get; set; }
        private Dictionary<string, string> SelectorNameAndType { get; set; }
        private string SortedColumnName { get; set; }

        public void AddKeySelector(Expression<Func<TEntity, string>> keySelector, string keyName)
        {
            StringKeySelectors.Add(keyName, keySelector);
            SelectorNameAndType[keyName] = "String";
        }

        public void AddKeySelector(Expression<Func<TEntity, DateTime>> keySelector, string keyName)
        {
            DatetimeKeySelector.Add(keyName, keySelector);
            SelectorNameAndType[keyName] = "DateTime";
        }

        public void AddKeySelector(Expression<Func<TEntity, int>> keySelector, string keyName)
        {
            IntKeySelectors.Add(keyName, keySelector);
            SelectorNameAndType[keyName] = "Integer";
        }

        public void AddKeySelector(Expression<Func<TEntity, float>> keySelector, string keyName)
        {
            FloatKeySelectors.Add(keyName, keySelector);
            SelectorNameAndType[keyName] = "Float";
        }

        public void AddKeySelector(Expression<Func<TEntity, double>> keySelector, string keyName)
        {
            DoubleKeySelectors.Add(keyName, keySelector);
            SelectorNameAndType[keyName] = "Double";
        }

        public void AddKeySelector(Expression<Func<TEntity, bool>> keySelector, string keyName)
        {
            BoolKeySelectors.Add(keyName, keySelector);
            SelectorNameAndType[keyName] = "Boolean";
        }

        public IQueryable<TEntity> Sort(IQueryable<TEntity> query, bool ascendingSort)
        {
            string keyName = this.SortedColumnName;

            if (!SelectorNameAndType.ContainsKey(keyName))
            {
                keyName = SelectorNameAndType.First().Key;
            }

            string type = SelectorNameAndType[keyName];

            switch (type)
            {
                case "String":
                    query = this.GetOrderSequence(query, ascendingSort, StringKeySelectors[keyName]);
                    break;
                case "Double":
                    query = this.GetOrderSequence(query, ascendingSort, DoubleKeySelectors[keyName]);
                    break;
                case "DateTime":
                    query = this.GetOrderSequence(query, ascendingSort, DatetimeKeySelector[keyName]);
                    break;
                case "Integer":
                    query = this.GetOrderSequence(query, ascendingSort, IntKeySelectors[keyName]);
                    break;
                case "Float":
                    query = this.GetOrderSequence(query, ascendingSort, FloatKeySelectors[keyName]);
                    break;
                case "Boolean":
                    query = this.GetOrderSequence(query, ascendingSort, BoolKeySelectors[keyName]);
                    break;
                default:
                    break;
            }

            return query;
        }

        private IQueryable<TEntity> GetOrderSequence<TValue>(IQueryable<TEntity> query, bool ascendingSort, Expression<Func<TEntity, TValue>> selector)
        {
            if (ascendingSort)
            {
                return query.OrderBy(selector);
            }
            else
            {
                return query.OrderByDescending(selector);
            }
        }
        
    }
}
