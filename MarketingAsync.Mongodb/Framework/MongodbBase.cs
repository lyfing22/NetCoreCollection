using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace MarketingAsync.Mongodb.Framework
{
    public abstract class MongodbBase<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        #region Query
        public abstract IQueryable<TEntity> Queryable();

        public virtual List<TEntity> GetAllList()
        {
            return Queryable().ToList();
        }

        public virtual Task<List<TEntity>> GetAllListAsync()
        {
            return Task.FromResult(GetAllList());
        }

        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable().Where(predicate).ToList();
        }

        public virtual Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(GetAllList(predicate));
        }

        public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(Queryable());
        }

        public abstract TEntity FirstOrDefault(TPrimaryKey id);

        public virtual Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return Task.FromResult(FirstOrDefault(id));
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable().FirstOrDefault(predicate);
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(FirstOrDefault(predicate));
        }

        #endregion

        #region Insert

        public abstract TEntity Insert(TEntity entity);
        public abstract Task<TEntity> InsertAsync(TEntity entity);


        public abstract IEnumerable<TEntity> InsertList(IEnumerable<TEntity> list);

        #endregion



        #region Delete

        public virtual void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            return Task.Run(() => Delete(entity.Id));
        }

        public abstract void Delete(TPrimaryKey id);

        public virtual Task DeleteAsync(TPrimaryKey id)
        {
            return Task.Run(() => Delete(id));
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in Queryable().Where(predicate).ToList())
            {
                Delete(entity);
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {

            await Task.Run(() => Delete(predicate));
        }

        #endregion

        #region Ad 

        public virtual int Count()
        {
            return Queryable().Count();
        }

        public virtual Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable().Where(predicate).Count();
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Count(predicate));
        }

        public virtual long LongCount()
        {
            return Queryable().LongCount();
        }

        public virtual Task<long> LongCountAsync()
        {
            return Task.FromResult(LongCount());
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable().Where(predicate).LongCount();
        }


        #endregion

        public abstract long Update(TPrimaryKey id, object fields);


    }
}