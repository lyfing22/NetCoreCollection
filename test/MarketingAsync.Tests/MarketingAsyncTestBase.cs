using System;
using System.Threading.Tasks;
using Abp.TestBase;
using MarketingAsync.Dapper;
using MarketingAsync.Mongodb;
using MarketingAsync.Redis.Cache;
using MarketingAsync.Tests.TestDatas;

namespace MarketingAsync.Tests
{
    public class MarketingAsyncTestBase : AbpIntegratedTestBase<MarketingAsyncTestModule>
    {
        public MarketingAsyncTestBase()
        {
            CacheConfigurage.ConnectionString = "10.0.3.9:6379,allowAdmin=True,password=crs-hpn2eric:XBiFRMv8MBgCiDhY";
            PersistentConfigurage.MasterConnectionString = "Server=10.0.3.8,1801;Database=tempdb_2018;User Id=Login_XG;Password=gSw24KCm";
            DocumentConfigurage.ConnectionString = "mongodb://127.0.0.1";
            DocumentConfigurage.DatabaseMongoDB = "fangwei_wxc_db";

        }
    }
}