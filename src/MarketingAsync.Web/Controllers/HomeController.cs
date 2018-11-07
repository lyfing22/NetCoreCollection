using System;
using System.Diagnostics;
using MarketingAsync.Act.Act;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.ActuatorApp;
using MarketingAsync.Dapper;
using MarketingAsync.Mongodb;
using MarketingAsync.Redis;
using MarketingAsync.Redis.Cache;
using Microsoft.AspNetCore.Mvc;

namespace MarketingAsync.Web.Controllers
{
    public class HomeController : MarketingAsyncControllerBase
    {


        private readonly ISignPointActivityRepository _signPointActivityRepository;
        private readonly ISignActivityRepository _signActivityRepository;
        private readonly IRedisHelper _redisHelper;
        private readonly IActuatorAppService _actuatorAppService;

        public HomeController(IActuatorAppService actuatorAppService, IRedisHelper redisHelper, ISignActivityRepository signActivityRepository, ISignPointActivityRepository signPointActivityRepository)
        {
            _actuatorAppService = actuatorAppService;
            _redisHelper = redisHelper;
            _signActivityRepository = signActivityRepository;
            _signPointActivityRepository = signPointActivityRepository;
        }


        public JsonResult Index()
        {
            var watch = new Stopwatch();
            watch.Start();
            _actuatorAppService.StartWork();
            watch.Stop();
            return Json(watch.ElapsedMilliseconds);
        }

        public JsonResult Index2(int id)
        {
            Console.WriteLine(id);
            _actuatorAppService.StartWorkFor(id);
            return Json("");
        }


        public JsonResult TestRedis()
        {
            var guid = Guid.NewGuid().ToString();
            _redisHelper.StringSet("guid:" + guid, guid, 1000);
            _redisHelper.StringGet("guid:" + guid);
            _redisHelper.KeyDelete("guid:" + guid);
            return Json(guid);
        }

        public JsonResult SetSignal(bool signalr)
        {
            SignalConfig.IsStop = signalr;
            return Json(signalr);
        }

        public JsonResult TestMongodb()
        {
            try
            {
                var count = _signActivityRepository.Count();
                Console.WriteLine("all count" + count);
                return Json(count);
            }
            catch (Exception e)
            {
                throw new Exception("Mongodb  ===>" + DocumentConfigurage.ConnectionString + " ex" + e.ToString());
            }
        }

        public JsonResult TestSqlServer()
        {
            try
            {
                var data = _signPointActivityRepository.Query<int>("select distinct top 1 id  from  SignPointActivity");

                return Json(data);
            }
            catch (Exception e)
            {
                throw new Exception("SqlServer  ===>" + PersistentConfigurage.MasterConnectionString + " ex" + e.ToString());
            }
        }
    }
}