using System;
using System.Linq;
using MarketingAsync.Act.Act;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.Dapper;
using MarketingAsync.Mongodb;
using MarketingAsync.Redis;
using MarketingAsync.Redis.Cache;
using Shouldly;
using Xunit;

namespace MarketingAsync.Tests.TestDatas
{
    public class DbTest : MarketingAsyncTestBase
    {

        private readonly ISignPointActivityRepository _signPointActivityRepository;
        private readonly ISignActivityRepository _signActivityRepository;
        private readonly IRedisHelper _redisHelper;


        public DbTest()
        {
            _signActivityRepository = LocalIocManager.Resolve<ISignActivityRepository>(); ;
            _redisHelper = LocalIocManager.Resolve<IRedisHelper>(); ;
            _signPointActivityRepository = LocalIocManager.Resolve<ISignPointActivityRepository>();
        }


        //[Fact]
        //public void TestRedis()
        //{
        //    var guid = Guid.NewGuid().ToString();
        //    _redisHelper.StringSet("guid:" + guid, guid, 1000);
        //    _redisHelper.StringGet("guid:" + guid).ShouldBe(guid, "redis数据库连接字符串有问题===>" + CacheConfigurage.ConnectionString);
        //}

        //[Fact]
        //public void TestMongodb()
        //{
        //    try
        //    {
        //        var count = _signActivityRepository.Count();
        //        Console.WriteLine("现存数据" + count);

        //    }
        //    catch (Exception e)
        //    {
        //        throw new TestDbException("Mongodb===>" + DocumentConfigurage.ConnectionString + " ex" + e.ToString());
        //    }
        //}

        //[Fact]
        //public void TestSqlServer()
        //{
        //    try
        //    {
        //        var data = _signPointActivityRepository.Query<int>("select distinct top 1 id  from  SignPointActivity");
        //        data.ShouldNotBe(null, "SqlServer 数据有问题 ===>" + DocumentConfigurage.ConnectionString);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new TestDbException("SqlServer===>" + PersistentConfigurage.MasterConnectionString + " ex" + e.ToString());
        //    }
        //}


    }
}