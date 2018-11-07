using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MarketingAsync.Act.Act;
using MarketingAsync.Act.Keys;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.Act.User;
using MarketingAsync.ActuatorApp;
using MarketingAsync.ActuatorApp.Dtos;
using MarketingAsync.Redis;
using Microsoft.EntityFrameworkCore.Internal;
using Shouldly;
using Xunit;

namespace MarketingAsync.Tests.TestApplication
{
    public class TestWorks : MarketingAsyncTestBase
    {


        private readonly IActuatorAppService _actuatorAppService;
        private readonly ISignPointActivityRepository _signPointActivityRepository;

        private readonly ISignActivityRepository _signActivityRepository;

        //营销活动奖品记录仓储
        private readonly IEventAwardRecordsRepository _eventAwardRecordsRepository;
        private readonly IRedisHelper _redisHelper;
        private readonly IUserSignActivityRepository _userSignActRepositoryMongo;
        private readonly ISignRecordsRepository _signRecordsRepository;
        private readonly string SUCCESSACTIDQUEUEKEY = "Export_SuccessActIdKey";

        public TestWorks()
        {
            _signActivityRepository = LocalIocManager.Resolve<ISignActivityRepository>();
            _signPointActivityRepository = LocalIocManager.Resolve<ISignPointActivityRepository>();
            _actuatorAppService = LocalIocManager.Resolve<IActuatorAppService>();
            _eventAwardRecordsRepository = LocalIocManager.Resolve<IEventAwardRecordsRepository>();
            _redisHelper = LocalIocManager.Resolve<IRedisHelper>();
            _signRecordsRepository = LocalIocManager.Resolve<ISignRecordsRepository>();
            _userSignActRepositoryMongo = LocalIocManager.Resolve<IUserSignActivityRepository>();
        }

        [Fact]
        public void IsSuccessfullyInsertData()
        {

            var actIdList = _signPointActivityRepository.Query<int>("select distinct top 1 id  from  SignPointActivity");
            var watch = new Stopwatch();
            watch.Start();
            //数据库应该有数据
            actIdList.ShouldNotBeNull();
            //条目不为空
            actIdList.Any().ShouldBe(true);

            var node = actIdList.FirstOrDefault();
            var boo = _actuatorAppService.ExportAct(node);
            boo.ShouldBe(true);
            //检测mongodb的字段
            List<IEnumerable<object>> queryList = _signPointActivityRepository.QueryMultiple(
                "select top 1 * from SignPointActivity where  ID=@id;select  * from   SignPointSet where  ActivityID=@id",
                new { id = node }, typeof(SignPointActivityDto), typeof(SignPointSetDto));
            SignPointActivityDto signAct = (SignPointActivityDto)queryList[0].FirstOrDefault();
            signAct.ShouldNotBeNull();
            List<SignPointSetDto> actSetList = new List<SignPointSetDto>();
            foreach (var item in queryList[1])
            {
                actSetList.Add((SignPointSetDto)item);
            }
            actSetList.Any().ShouldBe(true);

            SignActivity signMongo = _signActivityRepository.FirstOrDefault(x => x.Id == signAct.ActID.ToString());
            signMongo.ShouldNotBeNull();
            signMongo.Title.ShouldBe(signAct.Title);
            signMongo.BackGroundImg.ShouldBe(signAct.BackGroundImg);
            signMongo.BtnName.ShouldBe(signAct.BtnName);
            signMongo.BtnUrl.ShouldBe(signAct.BtnUrl);
            signMongo.BtnUrlName.ShouldBe(signAct.BtnUrlName);
            signMongo.CssStyle.ShouldBe(signAct.CssStyle);
            signMongo.FreeChance.ShouldBe(signAct.FreeChance);
            signMongo.RegistrationRemark.ShouldBe(signAct.RegistrationRemark);
            signMongo.Memberlogin.ShouldBe(signAct.Memberlogin);
            signMongo.SignPointSet.Count().ShouldBe(actSetList.Count());
            string timePattern = "yyyyMMdd";
            signMongo.BeginTime.ToString(timePattern).ShouldBe(signAct.BeginTime.ToString(timePattern));
            signMongo.EndTime.ToString(timePattern).ShouldBe(signAct.EndTime.ToString(timePattern));

            //检查奖项库存
            foreach (var item in signMongo.SignPointSet)
            {
                if (item.WxHbID > 0)//红包
                {
                    int prizeType = 0;
                    int activityType = item.IsSpecial == 0 ? 2 : 3;
                    string hbStockCountKey = string.Format(SignKeys.signactprizegrantnum_key, item.ID, prizeType);
                    hbStockCountKey = "B200016:" + hbStockCountKey;
                    int hbGrantCount = _eventAwardRecordsRepository.Count(" where MemberLogin=@Memberlogin and ActivityID=@ActID  and ActivityType=@ActivityType and (PrizeType=@PrizeType or  PrizeType=@PrizeType2 )  and AwardSetId = @AwardSetId",
                        new { Memberlogin = item.Memberlogin, ActID = node, ActivityType = activityType, PrizeType = 5, PrizeType2 = 6, AwardSetId = item.ID });
                    int count = Convert.ToInt32(_redisHelper.StringGet(hbStockCountKey));
                    count.ShouldBe(hbGrantCount);
                }
                if (item.ProductID > 0)//产品
                {
                    int prizeType = 1;
                    int activityType = item.IsSpecial == 0 ? 2 : 3;
                    string productStockCountKey = string.Format(SignKeys.signactprizegrantnum_key, item.ID, prizeType);
                    productStockCountKey = "B200016:" + productStockCountKey;
                    int prizeTypeDB = 0;
                    int productStockCount = _eventAwardRecordsRepository.Count("where MemberLogin=@Memberlogin and ActivityID=@ActID  and ActivityType=@ActivityType and PrizeType=@PrizeType  and AwardSetId = @AwardSetId",
                        new { Memberlogin = item.Memberlogin, ActID = node, ActivityType = activityType, PrizeType = prizeTypeDB, AwardSetId = item.ID });
                    int count = Convert.ToInt32(_redisHelper.StringGet(productStockCountKey));
                    count.ShouldBe(productStockCount);
                }
            }


            //检查用户数据
            IEnumerable<string> openIdList = _signRecordsRepository
                .Query<string>("select distinct  openid  from   SignRecords where  ActivityID=@actID ",
                    new { actID = node });
            if (openIdList == null || !openIdList.Any()) return;//无人参加
            string openid = openIdList.FirstOrDefault();
            List<IEnumerable<object>> queryList2 = _signPointActivityRepository.QueryMultiple(
                "select  *   from   SignRecords where  ActivityID=@actID and Openid=@openId order by ID desc; " +
                "select  *   from   SignSpecialRecords where  ActivityID=@actID and Openid=@openId; " +
                "select  *   from   MemberSignatureCard where  ActivityID=@actID and Openid=@openId; " +
                //"select  *   from   EventAwardRecords where  ActivityID=@actID and Openid=@openId and (ActivityType=@actType or ActivityType=@actSpecialType ",
                "select  *   from   EventAwardRecords where  ActivityID=@actID and Openid=@openId and ActivityType=@actType union  select  *   from   EventAwardRecords where  ActivityID=@actID and Openid=@openId and ActivityType=@actSpecialType ",
                new { actID = node, openId = openid, actType = 2, actSpecialType = 3 }, new Type[] { typeof(SignRecordsDto), typeof(SignSpecialRecordsDto), typeof(MemberSignatureCardDto), typeof(EventAwardRecordsDto) });

            //签到记录
            UserSignActivity userSignMongo =_userSignActRepositoryMongo.FirstOrDefault(UserSignActivity.GetId(openid, signMongo.Id));
            userSignMongo.ShouldNotBeNull();
            if (queryList2[0].Any())
            {
                List<SignRecordsDto> records = new List<SignRecordsDto>();
                foreach (var item in queryList2[0])
                {
                    records.Add((SignRecordsDto)item);
                }
                userSignMongo.SignRecords.Count().ShouldBe(records.Count());
                var recordDB = records.OrderBy(m => m.Addtime).FirstOrDefault();
                var recordMongo = userSignMongo.SignRecords.OrderBy(t => t.Addtime).FirstOrDefault();
                userSignMongo.Id.ShouldBe(UserSignActivity.GetId(openid, signMongo.Id));
                recordMongo.CustomerID.ShouldBe(recordDB.CustomerID);
                recordMongo.Day.ShouldBe(recordDB.Day);
                recordMongo.IsContinue.ShouldBe(recordDB.IsContinue);
                recordMongo.IsSignature.ShouldBe(recordDB.IsSignature);
                recordMongo.memberlogin.ShouldBe(recordDB.memberlogin);

            }
            //特殊
            if (queryList2[1].Any())
            {
                List<SignSpecialRecordsDto> specials = new List<SignSpecialRecordsDto>();
                foreach (var item in queryList2[1])
                {
                    specials.Add((SignSpecialRecordsDto)item);
                }
                userSignMongo.SignSpecialRecords.Count().ShouldBe(specials.Count);
                var recordDB = specials.OrderBy(m => m.Addtime).FirstOrDefault();
                var recordMongo = userSignMongo.SignSpecialRecords.OrderBy(t => t.Addtime).FirstOrDefault();
                recordMongo.SpecialSDate.ShouldBe(recordDB.SpecialSDate);
                recordMongo.CustomerID.ShouldBe(recordDB.CustomerID);

            }
            //补签卡
            if (queryList2[2].Any())
            {
                List<MemberSignatureCardDto> cards = new List<MemberSignatureCardDto>();
                foreach (var item in queryList2[2])
                {
                    cards.Add((MemberSignatureCardDto)item);
                }
                userSignMongo.MemberSignatureCard.Count().ShouldBe(cards.Count());
                userSignMongo.MemberSignatureCard.Count(m => m.IsUse == 1).ShouldBe(cards.Count(m => m.IsUse == 1));
            }
            //奖项
            if (queryList2[3].Any())
            {
                List<EventAwardRecordsDto> awards = new List<EventAwardRecordsDto>();
                foreach (var item in queryList2[3])
                {
                    awards.Add((EventAwardRecordsDto)item);
                }
                userSignMongo.EventAwardRecords.Count().ShouldBe(awards.Count());

            }

            //检测完删掉mongodb的测试数据
            //   _signActivityRepository.Delete 

            foreach (var user in openIdList)
            {
                _userSignActRepositoryMongo.Delete(UserSignActivity.GetId(user, signMongo.Id));
            }

            _signActivityRepository.Delete(signMongo.Id);


            watch.Stop();
            watch.ElapsedMilliseconds.ShouldBeLessThan(10000);


        }

        //重置缓存
        //_redisHelper.StringSet(SUCCESSACTIDQUEUEKEY, "0", 60 * 60 * 24 * 3);
        //    watch.Stop();
        //    watch.ElapsedMilliseconds.ShouldBeLessThan(10000);

        //}


        //重复数据测试
        [Fact]
        public void IsSuccessDeleteData()
        {
            IEnumerable<SignPointActivityDto> actIdList =
                _signPointActivityRepository.Query<SignPointActivityDto>("select top  2  *  from  SignPointActivity");
            //数据库应该有数据
            actIdList.ShouldNotBeNull();
            //条目不为空
            actIdList.Any().ShouldBe(true);
            foreach (var act in actIdList)
            {
                var watch = new Stopwatch();
                watch.Start();
                _actuatorAppService.StartWorkFor(act.Id);
                _actuatorAppService.StartWorkFor(act.Id);
                ;
                _signActivityRepository.Count(t => t.ActID == act.ActID.ToString()).ShouldBe(1);
                watch.Stop();
                watch.ElapsedMilliseconds.ShouldBeLessThan(10000);
            }

            foreach (var act in actIdList)
            {
                _actuatorAppService.DeleteActData(act.Id, act.ActID.ToString());
            }
            //重置缓存
            _redisHelper.KeyDelete(SUCCESSACTIDQUEUEKEY);
            _redisHelper.StringGet(SUCCESSACTIDQUEUEKEY).ShouldBe(null);

        }



        //数据第二次连续导入测试
        [Fact]
        public void IsSuccessSecondExport()
        {
            //准备环境
            IEnumerable<SignPointActivityDto> actIdList =
                _signPointActivityRepository.Query<SignPointActivityDto>("select top  20  *  from  SignPointActivity");
            actIdList.ShouldNotBeNull();
            actIdList.Any().ShouldBe(true);
            foreach (var act in actIdList)
            {
                _actuatorAppService.StartWorkFor(act.Id);
                _redisHelper.StringSet(SUCCESSACTIDQUEUEKEY, act.Id.ToString(), 60 * 60 * 24 * 3);
            }

            var entity = actIdList.ToList().GetRange(16, 1).FirstOrDefault();
            _redisHelper.StringSet(SUCCESSACTIDQUEUEKEY, entity.Id.ToString(), 60 * 60 * 24 * 3);

            var watch = new Stopwatch();
            watch.Start();

            //返回现场
            _actuatorAppService.ExcuteWorker(actIdList.Select(x => x.Id));

            //检测数据条目有没有跟数据库条目一致


            string actStr = _redisHelper.StringGet(SUCCESSACTIDQUEUEKEY);
            int actId = 0;
            if (!string.IsNullOrEmpty(actStr))
            {
                actId = Convert.ToInt32(actStr);
            }

            //缓存中的ID为最终添加的记录
            int mongoDbId = _signActivityRepository.Queryable().Max(t => t.RowId);
            actId.ShouldBe(actIdList.Max(t => t.Id));
            mongoDbId.ShouldBe(actId);


            //MongoDb中没有相关记录
            IEnumerable<SignPointActivityDto> actList =
                _signPointActivityRepository.Query<SignPointActivityDto>(
                    "select top  2  *  from  SignPointActivity where id>@id", new { id = actId });
            foreach (var actItem in actList)
            {
                //活动数据不存在
                var actMongo = _signActivityRepository.FirstOrDefault(actItem.ActID.ToString());
                actMongo.ShouldBeNull();

                //用户数据不存在
                IEnumerable<string> openIdList = _signRecordsRepository
                    .Query<string>("select distinct  openid  from   SignRecords where  ActivityID=@actID ",
                        new { actID = actItem.Id });
                foreach (var openId in openIdList)
                {
                    //活动数据不存在
                    UserSignActivity actMongoTemp =
                        _userSignActRepositoryMongo.FirstOrDefault(UserSignActivity.GetId(openId,
                            actItem.ActID.ToString()));
                    actMongoTemp.ShouldBeNull();
                }
            }

            //清理测试数据
            foreach (var act in actIdList)
            {
                _actuatorAppService.DeleteActData(act.Id, act.ActID.ToString());
            }

            //重置缓存
            _redisHelper.StringSet(SUCCESSACTIDQUEUEKEY, "0", 60 * 60 * 24 * 3);
            watch.Stop();
            watch.ElapsedMilliseconds.ShouldBeLessThan(10000);

        }

        /// <summary>
        /// 检测删除脏数据是否成功
        /// </summary>
        public void TestCheckDeleteBadDataIsSuccessfully()
        {
            //准备环境
            IEnumerable<SignPointActivityDto> actIdList =
                _signPointActivityRepository.Query<SignPointActivityDto>("select top  20  *  from  SignPointActivity");
            actIdList.ShouldNotBeNull();
            actIdList.Any().ShouldBe(true);
            //
            foreach (var act in actIdList)
            {
                _actuatorAppService.StartWorkFor(act.Id);
                _redisHelper.StringSet(SUCCESSACTIDQUEUEKEY, act.Id.ToString(), 60 * 60 * 24 * 3);
            }

            var rid = actIdList.ToList().GetRange(16, 1).FirstOrDefault().Id;
            //强制缓存在17条的位置停止
            _redisHelper.StringSet(SUCCESSACTIDQUEUEKEY, rid.ToString(), 60 * 60 * 24 * 3);

            //执行返回现场
            _actuatorAppService.BackToLastExport();


            //mongodb为缓存中最终添加的记录
            int mongoDbId = _signActivityRepository.Queryable().Max(t => t.RowId);
            mongoDbId.ShouldBe(rid);


            //MongoDb中没有相关记录
            IEnumerable<SignPointActivityDto> actList =
                _signPointActivityRepository.Query<SignPointActivityDto>(
                    "select top  3  *  from  SignPointActivity where id>@id", new { id = rid });
            foreach (var actItem in actList)
            {
                //活动数据不存在
                var actMongo = _signActivityRepository.FirstOrDefault(actItem.ActID.ToString());
                actMongo.ShouldBeNull();

                //用户数据不存在
                IEnumerable<string> openIdList = _signRecordsRepository
                    .Query<string>("select distinct  openid  from   SignRecords where  ActivityID=@actID ",
                        new { actID = actItem.Id });
                foreach (var openId in openIdList)
                {
                    //活动数据不存在
                    UserSignActivity actMongoTemp =
                        _userSignActRepositoryMongo.FirstOrDefault(UserSignActivity.GetId(openId,
                            actItem.ActID.ToString()));
                    actMongoTemp.ShouldBeNull();
                }
            }

            var rids = actIdList.Select(x => x.Id);
            var actids = actIdList.Select(x => x.ActID.ToString());
            //清理测试数据
            foreach (var act in actIdList)
            {
                _actuatorAppService.DeleteActData(act.Id, act.ActID.ToString());
            }
            //检测活动数据是否清除成功
            _signActivityRepository.Count(x => rids.Contains(x.RowId)).ShouldBe(0);
            _userSignActRepositoryMongo.Count(x => actids.Contains(x.ActivityId));


        }

    }
}