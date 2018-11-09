
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

            CacheConfigurage.ConnectionString = "192.168.5.30:6379,allowAdmin=True,password=miduo2018DEV";
            PersistentConfigurage.MasterConnectionString = "Server=192.168.5.138,16411;Database=fangwei_wxc_db;User Id=test2016;Password=test2016";
            DocumentConfigurage.ConnectionString = "mongodb://192.168.5.72";
            DocumentConfigurage.DatabaseMongoDB = "fangwei_wxc_db";

        }

    }
}
