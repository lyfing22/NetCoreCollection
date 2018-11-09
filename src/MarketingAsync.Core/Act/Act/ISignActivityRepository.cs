using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abp.Dependency;
using MarketingAsync.Act.SigException;
using MongoDB.Bson;

namespace MarketingAsync.Act.Act
{
    /// <summary>
    /// 活动仓储
    /// </summary>
    public interface ISignActivityRepository : IMongoRepository<SignActivity>, ISingletonDependency
    {
        Tuple<long,long> ClearData(bool error);
    }
}