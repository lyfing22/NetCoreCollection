using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.Dapper.Framework;

namespace MarketingAsync.Dapper.Repository
{
 public    class SignCardStaticRepository : DapperRepository<fact_Statistic_SignatureCard>, ISignCardStaticRepository
    {
        public override T ConnectionExcute<T>(Func<IDbConnection, T> func)
        {
            var connection = string.Format(PersistentConfigurage.NotDatabase, PersistentConfigurage.CardStaticDBName);
            return base.ConnectionExcute(func, connection);
        }
    }
}
