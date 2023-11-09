using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BaseProject.DAO.IRepository;
using BaseProject.DAO.Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BaseProject.DAO.Repository
{

    public abstract class Repository<TEntity, TContext> : IDisposable, IRepository<TEntity, TContext>
    where TEntity : class
    where TContext : IdentityDbContext<AspNetUser>, new()
    {
        internal TContext context;
        internal DbSet<TEntity> set;
        internal int bulkCount;

        public Repository(TContext context)
        {
            this.UpdateContext(context);
        }

        public Repository()
        {
            this.UpdateContext(new TContext());
        }

        private void UpdateContext(TContext context)
        {
            this.context = context;
            this.set = context.Set<TEntity>();
            this.bulkCount = 0;
        }

        protected TContext Context
        {
            get
            {
                return context;
            }
        }

        public DbSet<TEntity> Set
        {
            get
            {
                return this.set;
            }
        }

        public virtual TEntity GetByID(object id)
        {
            TEntity entity = null;

            try
            {
                entity = set.Find(id);
            }
            catch (Exception e) { }

            return entity;
        }


        public virtual async Task<TEntity> GetByIDAsync(object id)
        {
            TEntity entity = null;

            try
            {
                entity = await set.FindAsync(id);
            }
            catch (Exception) { }

            return entity;
        }

        public virtual bool Insert(TEntity entity)
        {
            try
            {
                set.Add(entity);
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                //context.Dispose();
                return false;
            }
        }
        public virtual bool InsertTrans(TEntity entity)
        {
            try
            {
                set.Add(entity);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual async Task<bool> InsertAsync(TEntity entity)
        {
            set.Add(entity);

            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual bool InsertAll(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
            {
                return true;
            }

            try
            {
                for (int i = 0; i < entities.Count(); i++)
                {
                    context.Entry(entities.ElementAt(i)).State = EntityState.Added;
                }

                context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public virtual bool InsertAllTrans(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
            {
                return true;
            }

            try
            {
                set.AddRange(entities);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual async Task<bool> InsertAllAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
            {
                return true;
            }

            try
            {
                set.AddRange(entities);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual bool Delete(int id)
        {
            TEntity entity = set.Find(id);
            return Delete(entity);
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            TEntity entity = set.Find(id);
            return await DeleteAsync(entity);
        }

        public virtual bool DeleteByFilter(Expression<Func<TEntity, bool>> filter)
        {
            TEntity[] toDelete = set.Where(filter).ToArray();

            try
            {
                set.RemoveRange(toDelete);
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public virtual async Task<bool> DeleteByFilterAsync(Expression<Func<TEntity, bool>> filter)
        {
            TEntity[] entities = set.Where(filter).ToArray();

            try
            {
                set.RemoveRange(entities);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual bool Delete(TEntity entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                set.Attach(entity);
            }

            try
            {
                set.Remove(entity);
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                set.Attach(entity);
            }

            try
            {
                set.Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Filtro criado para atender a especificação do datatables
        /// </summary>
        /// <param name="initialPosition">Posição começando do zero do primeiro valor da pagina</param>
        /// <param name="itensPerPage">Itens por pagina</param>
        /// <param name="query">Filtro</param>
        /// <param name="total">Numero de registro no banco</param>
        /// <returns>Todos os registros desta pagina</returns>
        public virtual TEntity[] Filter(
            IQueryable<TEntity> query,
            int initialPosition,
            int itensPerPage,
            out int total,
            string includeProperties = ""
        )
        {
            foreach (var includeProperty in includeProperties.Split
            (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            total = query.Count();
            query = query.AsNoTracking();
            query = query.Skip(initialPosition);
            query = query.Take(itensPerPage);

            return query.ToArray();
        }

        public virtual T[] FilterSelect2<T>(
            Expression<Func<TEntity, T>> keySelector,
            IQueryable<TEntity> query,
            int initialPosition,
            int itensPerPage,
            out int total,
            string includeProperties = ""
        )
        {
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            query = query.AsNoTracking();
            total = query.Count();
            query = query.Skip(initialPosition);
            query = query.Take(itensPerPage);

            return query.Select(keySelector).ToArray();
        }

        public virtual bool DeleteAll(IEnumerable<TEntity> entities)
        {
            if (entities.Count() == 0)
            {
                return true;
            }

            int entityCount = entities.Count();
            for (int i = 0; i < entityCount; i++)
            {

                if (context.Entry(entities.ElementAt(i)).State == EntityState.Detached)
                {
                    set.Attach(entities.ElementAt(i));
                }
            }

            set.RemoveRange(entities);

            try
            {
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public virtual bool DeleteAllTrans(IEnumerable<TEntity> entities)
        {
            if (entities.Count() == 0)
            {
                return true;
            }

            int entityCount = entities.Count();
            for (int i = 0; i < entityCount; i++)
            {

                if (context.Entry(entities.ElementAt(i)).State == EntityState.Detached)
                {
                    set.Attach(entities.ElementAt(i));
                }
            }

            set.RemoveRange(entities);

            try
            {
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual async Task<bool> DeleteAllAsync(IEnumerable<TEntity> entities)
        {
            if (entities.Count() == 0)
            {
                return true;
            }

            int entityCount = entities.Count();
            for (int i = 0; i < entityCount; i++)
            {

                if (context.Entry(entities.ElementAt(i)).State == EntityState.Detached)
                {
                    set.Attach(entities.ElementAt(i));
                }
            }

            set.RemoveRange(entities);

            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual bool Update(TEntity entity)
        {
            try
            {
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual bool UpdateTrans(TEntity entity)
        {
            try
            {
                context.Entry(entity).State = EntityState.Modified;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            try
            {
                context.Entry(entity).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual bool UpdateOutsideContext(TEntity entity)
        {
            var current = set.Find(entity.GetType().GetProperty("Id").GetValue(entity, null));
            context.Entry(current).CurrentValues.SetValues(entity);
            context.Entry(current).State = EntityState.Modified;
            try
            {
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual bool UpdateAll(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
            {
                return true;
            }

            try
            {
                for (int i = 0; i < entities.Count(); i++)
                {
                    context.Entry(entities.ElementAt(i)).State = EntityState.Modified;
                }

                context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual async Task<bool> UpdateAllAsync(IEnumerable<TEntity> entities)
        {
            int entitiesCount = entities.Count();

            if (entitiesCount == 0) return true;

            try
            {
                for (int i = 0; i < entitiesCount; i++)
                {
                    context.Entry(entities.ElementAt(i)).State = EntityState.Modified;
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual bool UpdateAllOutsideContext(IEnumerable<TEntity> entities)
        {
            int entitiesCount = entities.Count();

            try
            {
                for (int i = 0; i < entitiesCount; i++)
                {
                    var current = set.Find(entities.ElementAt(i).GetType().GetProperty("Id").GetValue(entities.ElementAt(i), null));
                    context.Entry(current).CurrentValues.SetValues(entities.ElementAt(i));
                    context.Entry(current).State = EntityState.Modified;
                }

                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private PropertyInfo GetPropertyId(TEntity entity)
        {
            var properties = entity.GetType().GetProperties()
                                .Where(property => Regex.IsMatch(property.Name, @"^Id$"))
                                    .ToArray();

            PropertyInfo propertyId = null;

            int propertyCount = properties.Count();

            for (int i = 0; i < propertyCount; i++)
            {
                propertyId = properties.ElementAt(i);
            }

            return propertyId;
        }

        /// <summary>
        ///     Atualiza uma ICollection de elementos e separa quais registros
        ///     da ICollection devem ser excluidos ou adicionados da tabela associativa
        ///     Para excluir o modelo que deve ser enviado para o backend deve ter no registro
        ///     as chaves estrangeiras zeradas.
        ///     Para incluir basta que o Id seja 0.
        /// </summary>
        /// <param name="entities">ICollection</param>
        /// <returns>true</returns>
        public virtual bool UpdateMany(ICollection<TEntity> entities)
        {
            if (entities.Count == 0)
            {
                return true;
            }

            var type = entities.First().GetType();

            var properties = type.GetProperties();

            PropertyInfo propertyId = null;

            PropertyInfo foreignId1 = null;

            PropertyInfo foreignId2 = null;

            int propertyCount = properties.Count();
            for (int i = 0; i < propertyCount; i++)
            {
                if (Regex.IsMatch(properties.ElementAt(i).Name, @"^Id$"))
                {
                    propertyId = properties.ElementAt(i);
                }
                else if (Regex.IsMatch(properties.ElementAt(i).Name, @"^Id[A-Z]"))
                {
                    if (foreignId1 == null)
                    {
                        foreignId1 = properties.ElementAt(i);
                    }
                    else
                    {
                        foreignId2 = properties.ElementAt(i);
                        break;
                    }
                }
            }

            var entitiesToInsert = new List<TEntity>();

            if (propertyId != null)
            {
                entitiesToInsert =
                    (from entity in entities
                     let id = (int)propertyId.GetValue(entity, null)
                     where id == 0
                     select entity).ToList();
            }

            var entitiesToDelete = new List<TEntity>();

            if (foreignId1 != null && foreignId2 != null)
            {
                entitiesToDelete = (from entity in entities
                                    let id1 = (int?)foreignId1.GetValue(entity, null)
                                    let id2 = (int?)foreignId2.GetValue(entity, null)
                                    where id1.HasValue && id1 == 0 && id2.HasValue && id2 == 0
                                    select entity).ToList();
            }

            var sucesso = InsertAll(entitiesToInsert.AsEnumerable());
            sucesso = sucesso && DeleteAll(entitiesToDelete);

            return sucesso;
        }

        public virtual T[] Select<T>(
            Expression<Func<TEntity, T>> keySelector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool noTracking = false
        )
        {
            IQueryable<TEntity> query = this.set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            IQueryable<T> selectedQuery = query.Select(keySelector);

            return selectedQuery.ToArray();
        }

        public virtual async Task<T[]> SelectAsync<T>(
            Expression<Func<TEntity, T>> keySelector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
        )
        {
            IQueryable<TEntity> query = this.set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            IQueryable<T> selectedQuery = query.Select(keySelector);

            return await selectedQuery.ToArrayAsync();
        }

        public virtual TEntity FirstOrDefault(
            Expression<Func<TEntity, bool>> filter,
            string includeProperties = "",
            bool noTracking = false,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
        )
        {
            var query = this.set.AsQueryable();

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query.FirstOrDefault(filter);
        }

        public virtual bool Exists(Expression<Func<TEntity, bool>> filter)
        {
            var query = this.set.AsQueryable();

            query = query.AsNoTracking();

            return query.Any(filter);
        }

        public virtual TEntity[] Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool noTracking = false)
        {
            IQueryable<TEntity> query = this.set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return query.ToArray();

        }

        public virtual TEntity[] GetBulk(
            Expression<Func<TEntity, bool>> filter,
            int limit,
            string includeProperties = "",
            bool noTracking = false)
        {
            IQueryable<TEntity> query = this.set;

            query = query.Where(filter);

            query = query.Take(limit);

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return query.ToArray();

        }

        public virtual async Task<TEntity[]> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool noTracking = false)
        {
            IQueryable<TEntity> query = this.set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToArrayAsync();
        }

        /// <summary>
        /// Attach an entity to context as it was recently taken form db
        /// now all changes made on this will be tracked
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual EntityEntry<TEntity> AttachEntityToContext(TEntity entity)
        {
            return set.Attach(entity);
        }

        /// <summary>
        /// Get the set from database for a more close linq operations on db
        ///  but now you will be susceptible to exceptions thrown by it
        /// </summary>
        /// <returns>Returns the Dbset</returns>
        public virtual DbSet<TEntity> Query()
        {
            return set;
        }

        public bool IsEmpty()
        {
            return !set.Any();
        }

        public TEntity GetFirst(Expression<Func<TEntity, bool>> expressao = null)
        {
            IQueryable<TEntity> query = set;

            if (expressao != null)
            {
                query = query.Where(expressao);
            }

            return query.FirstOrDefault();
        }

        public virtual bool SaveChanges()
        {
            try
            {
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return e == null;
            }
        }

        public virtual async Task<bool> SaveChangesAsync()
        {
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return e == null;
            }
        }

        public TContext GetContext()
        {
            return context;
        }

        public void Dispose()
        {
            this.context.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual bool CanBeUpdate(TEntity entity)
        {
            var collections = entity.GetType().GetProperties().Where(x => x.PropertyType.IsAbstract).ToArray();

            int collectionsCount = collections.Count();

            for (int i = 0; i < collectionsCount; i++)
            {
                var collection = collections[i];
                var value = collection.GetValue(entity);
                var propertCount = value.GetType().GetProperties().FirstOrDefault();
                var associatedData = propertCount.GetValue(value);

                if ((int)associatedData > 0)
                {
                    return false;
                };
            }

            return true;
        }

    }
}
