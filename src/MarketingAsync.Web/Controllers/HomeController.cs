using System;
using System.Diagnostics;
using System.Linq;
using MarketingAsync.Act.Act;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.ActuatorApp;
using MarketingAsync.Dapper;
using MarketingAsync.Mongodb;
using MarketingAsync.Mtimes;
using MarketingAsync.Redis;
using MarketingAsync.Redis.Cache;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MarketingAsync.Web.Controllers
{
    public class HomeController : MarketingAsyncControllerBase
    {

        /*
          print(db.SignActivity.count()+"|"+ db.UserSignActivity.count());
         */
        private readonly ISignPointActivityRepository _signPointActivityRepository;
        private readonly ISignActivityRepository _signActivityRepository;
        private readonly IRedisHelper _redisHelper;
        private readonly IActuatorAppService _actuatorAppService;

        private readonly string SUCCESSACTIDQUEUEKEY = "Export_SuccessActIdKey";

        public HomeController(
            IRedisHelper redisHelper,
            IActuatorAppService actuatorAppService,
            ISignActivityRepository signActivityRepository,
            ISignPointActivityRepository signPointActivityRepository
        )
        {
            _actuatorAppService = actuatorAppService;
            _redisHelper = redisHelper;
            _signActivityRepository = signActivityRepository;
            _signPointActivityRepository = signPointActivityRepository;
        }


        public JsonResult Index()
        {
            return Json("start");
        }

        public JsonResult StartWork()
        {
            _actuatorAppService.StartWork();
            return Json("start");
        }


        /// <summary>
        /// 删除mongodb和redis数据
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public JsonResult ClearMongodbAndRedis(bool error)
        {
            _actuatorAppService.DelectRedisCount();

            var dt = _signActivityRepository.ClearData(error);
            return Json(string.Format("delete active:{0},delete user{1}", dt.Item1, dt.Item2));
        }

        /// <summary>
        /// 删除mongodb
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public JsonResult ClearMongodb(bool error)
        {
            var dt = _signActivityRepository.ClearData(error);
            return Json(string.Format("delete active:{0},delete user{1}", dt.Item1, dt.Item2));
        }

        /// <summary>
        /// 删除Redis
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public JsonResult ClearRedis(bool error)
        {
            _actuatorAppService.DelectRedisCount();

            return Json("true");
        }


        public JsonResult InsertOne(int id)
        {
            Console.WriteLine(id);
            _actuatorAppService.StartWorkFor(id);
            return Json("");
        }

        /// <summary>
        /// 获取当前统计数据最后一行
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMarkLastLine()
        {
            var now = TimeControl.DataArray.FirstOrDefault().Value;
            var data = now[now.Count - 1];
            return Json(data);
        }

        /// <summary>
        /// 根据下标获取统计信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMarkForLength(int start, int end)
        {
            var now = TimeControl.DataArray.FirstOrDefault().Value;
            if (start > now.Count || end > now.Count)
            {
                return Json("长度超过下标");
            }

            if (start >= end)
            {
                return Json("起始值需小于结束值");
            }

            var data = now.GetRange(start, start + end - start);
            return Json(data);
        }

        /// <summary>
        /// 根据最后的几条数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLastMark(int count)
        {
            var now = TimeControl.DataArray.FirstOrDefault().Value;
            var start = now.Count - count;
            var data = now.GetRange(start, count);
            return Json(data);
        }



        public JsonResult TestRedis()
        {
            var guid = Guid.NewGuid().ToString();
            _redisHelper.StringSet("guid:" + guid, guid, 1000);
            _redisHelper.StringGet("guid:" + guid);
            _redisHelper.KeyDelete("guid:" + guid);
            return Json(guid);
        }


        public string SetRedis(string value)
        {
            _redisHelper.StringSet(SUCCESSACTIDQUEUEKEY, value, 60 * 60 * 24 * 3);
            return _redisHelper.StringGet(SUCCESSACTIDQUEUEKEY);
        }

        public override JsonResult Json(object obj)
        {
            return Json(obj, new JsonSerializerSettings()
            {

            });
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