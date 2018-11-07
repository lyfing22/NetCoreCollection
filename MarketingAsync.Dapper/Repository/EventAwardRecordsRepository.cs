using System;
using System.Collections.ObjectModel;
using System.Data;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.Dapper.Framework;

namespace MarketingAsync.Dapper.Repository
{
    public class EventAwardRecordsRepository : DapperRepository<EventAwardRecords>, IEventAwardRecordsRepository
    {
        //public override T ConnectionExcute<T>(Func<IDbConnection, T> func)
        //{
        //    var connection = string.Format(PersistentConfigurage.NotDatabase, "fangwei_wxc_db"); 
        //    return base.ConnectionExcute(func,connection);
        //}


    }
}