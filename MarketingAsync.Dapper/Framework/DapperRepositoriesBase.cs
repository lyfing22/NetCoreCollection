using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace MarketingAsync.Dapper.Framework
{
    public abstract class DapperRepositoriesBase<TEntity, T> : DapperRepository, IDapperRepository<TEntity, T>
        where TEntity : IEntity<T>
    {
        #region query  

        public abstract List<TEntity> GetList(string where, object parament = null, string field = "*");

        public virtual Task<List<TEntity>> GetAllListAsync(string where, object parament = null, string field = "*")
        {
            return Task.FromResult(GetList(where, parament, field));
        }

        public abstract TEntity Get(T id1);

        public virtual Task<TEntity> GetAsync(T id)
        {
            return Task.Run(() => Get(id));
        }

        public abstract TEntity FirstOrDefault(string where, object parament = null, string field = "*");

        public virtual Task<TEntity> FirstOrDefaultAsync(string where, object parament = null, string field = "*")
        {
            return Task.FromResult(FirstOrDefault(where, parament, field));
        }

        #endregion

        #region insert

        public abstract TEntity Insert(TEntity entity);

        public virtual Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Insert(entity));
        }

        #endregion

        #region update

        public abstract TEntity Update(TEntity entity);

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        #endregion

        #region delete


        public virtual void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public abstract void Delete(IEnumerable<TEntity> entities);

        public virtual Task DeleteAsync(TEntity entity)
        {
            return Task.Run(() => Delete(entity));
        }


        public abstract void Delete(T id);

        public virtual Task DeleteAsync(T id)
        {
            return Task.Run(() => Delete(id));
        }

        public abstract void Delete(string where, object parament = null);

        public virtual async Task DeleteAsync(string where, object parament = null)
        {
            await Task.Run(() => Delete(where, parament));
        }

        #endregion

        #region Aggregation 

        public abstract int Count();

        public virtual Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        public abstract int Count(string where, object parament = null);

        public virtual Task<int> CountAsync(string where, object parament = null)
        {
            return Task.FromResult(Count(where, parament));
        }

        #endregion
    }

    public abstract class DapperRepositoriesBase<TEntity> : DapperRepository<TEntity, int>,
        IDapperRepository<TEntity> where TEntity : Entity<int>, IEntity
    {


    }

}