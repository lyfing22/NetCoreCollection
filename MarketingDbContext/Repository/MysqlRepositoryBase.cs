using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;

namespace MarketingAsync.EFCore.Repository
{
    public class MysqlRepositoryBase<TEntity, T> : MarketingDbContext where TEntity : IEntity<T>
    {
       
    }
}
