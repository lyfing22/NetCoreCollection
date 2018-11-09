using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;

namespace MarketingAsync.Act
{
    public interface IMongoRepository<TEntity> : ITransientDependency where TEntity : class, IEntity<string>
    {

        #region Select/Get/Query 
        IQueryable<TEntity> Queryable();
        List<TEntity> GetAllList();
        Task<List<TEntity>> GetAllListAsync();
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);
        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);
        TEntity FirstOrDefault(string id);
        Task<TEntity> FirstOrDefaultAsync(string id);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion
        #region Insert 

        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        IEnumerable<TEntity> InsertList(IEnumerable<TEntity> list);

        #endregion

        #region Delete 

        /// <param name="entity"></param>
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
        void Delete(string id);
        Task DeleteAsync(string id);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Aggregates 
        int Count();
        Task<int> CountAsync();
        int Count(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        long LongCount();
        Task<long> LongCountAsync();

        #endregion

    }

}