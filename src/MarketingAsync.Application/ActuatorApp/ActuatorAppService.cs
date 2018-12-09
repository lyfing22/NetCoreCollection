using Abp.AutoMapper;
using AutoMapper;
using MarketingAsync.Act.Act;
using MarketingAsync.Act.Keys;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.Act.User;
using MarketingAsync.ActuatorApp.Dtos;
using MarketingAsync.Mtimes;
using MarketingAsync.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketingAsync.ActuatorApp
{
    public class ActuatorAppService : MarketingAsyncAppServiceBase, IActuatorAppService
    {

        //活动仓储
        private readonly ISignPointActivityRepository _signPointActivityRepository;
        //活动设置仓储
        private readonly ISignPointSetRepository _pointSetRepository;
        //签到记录表仓储
        private readonly ISignRecordsRepository _signRecordsRepository;
        //会员补签卡仓储
        private readonly IMemberSignatureCardRepository _memberSignatureCardRepository;
        //营销活动奖品记录仓储
        private readonly IEventAwardRecordsRepository _eventAwardRecordsRepository;
        //活动特殊记录仓储
        private readonly ISignSpecialRecordsRepository _signSpecialRecordsRepository;
        //
        private readonly ISignActivityRepository _signActRepositoryMongo;
        private readonly IUserSignActivityRepository _userSignActRepositoryMongo;
        private readonly IRedisHelper _redisHelper;

        private readonly ISignCardStaticRepository _signCardStaticRepository;
        private readonly IExportErrorRepository _exportErrorRepository;

        private readonly string SUCCESSACTIDQUEUEKEY = "Export_SuccessActIdKey";


        //private readonly MergeSignActivityDomainService _mergeSignActivity;

        public ActuatorAppService(
            ISignPointSetRepository pointSetRepository,
            ISignRecordsRepository signRecordsRepository,
            IMemberSignatureCardRepository memberSignatureCardRepository,
            IEventAwardRecordsRepository eventAwardRecordsRepository,

            ISignSpecialRecordsRepository signSpecialRecordsRepository,
        ISignPointActivityRepository signPointActivityRepository,
            MergeSignActivityDomainService mergeSignActivity,
            ISignActivityRepository signActRepositoryMongo,
            IUserSignActivityRepository userSignActRepositoryMongo,
            IRedisHelper redisHelper,
            ISignCardStaticRepository signCardStaticRepository,
            IExportErrorRepository exportErrorRepository)
        {
            _pointSetRepository = pointSetRepository;
            _signRecordsRepository = signRecordsRepository;
            _memberSignatureCardRepository = memberSignatureCardRepository;
            _eventAwardRecordsRepository = eventAwardRecordsRepository;
            _signSpecialRecordsRepository = signSpecialRecordsRepository;
            _signPointActivityRepository = signPointActivityRepository;
            _signActRepositoryMongo = signActRepositoryMongo;
            _userSignActRepositoryMongo = userSignActRepositoryMongo;
            _redisHelper = redisHelper;
            _signCardStaticRepository = signCardStaticRepository;
            _exportErrorRepository = exportErrorRepository;
            //_mergeSignActivity = mergeSignActivity;
        }

        public enum State
        {
            Start,
            End
        }


        TimeControl time = new TimeControl(Guid.NewGuid().ToString().Replace("-", ""));

        /// <summary>
        /// 设置耗时项
        /// </summary>
        /// <param name="index"></param>
        /// <param name="state"></param>
        public void SetStatisticalInformation(string index, State state)
        {

            return;
        }

        public void StartWork()
        {

            //time.Marke("1.启动锚点");
            int lastExportID = 0;
            string actStr = _redisHelper.StringGet(SUCCESSACTIDQUEUEKEY);
            if (!string.IsNullOrEmpty(actStr))
            {
                lastExportID = Convert.ToInt32(actStr);
            }

            //time.Marke("2.redis读取完毕");
            IEnumerable<int> actIdList = _signPointActivityRepository.Query<int>("select distinct  id  from  SignPointActivity where id>@rid", new { rid = lastExportID });

            //time.Marke("3.redis 执行 select distinct  id  from  SignPointActivity where id>@rid 查询完毕");


            //time.Marke("4.执行工作任务");
            //执行任务
            ExcuteWorker(actIdList);

            //time.Marke("5.工作任务执行完毕");

            Console.WriteLine("运行完毕, 请清理活动表（SignActivity）下的RowId 。。。");
            time.SaveData();

        }

        public void ExcuteWorker(IEnumerable<int> rouIdList)
        {
            //返回现场
            BackToLastExport();
            foreach (var rid in rouIdList)
            {
                try
                {
                    if (SignalConfig.IsStop)
                    {
                        Console.WriteLine("收到停止导入命令，当前导入id(当前id未导入):" + rid);
                        return;
                    }
                    //time.Marke("6.导入活动数据+用户数据", rid.ToString());
                    if (ExportAct(rid))
                    {
                        //time.Marke("7.设置redisKey", rid.ToString());
                        _redisHelper.StringSet(SUCCESSACTIDQUEUEKEY, rid.ToString(), 60 * 60 * 24 * 3);
                    }
                    else
                    {
                        //time.Marke("8.该活动导入数据失败了", rid.ToString());
                        Console.WriteLine(rid + "该活动导入数据失败了");
                        //加入日志
                        ExportError error = new ExportError()
                        {
                            RowId = rid.ToString(),
                            ErrorMsg = "该活动导入数据失败了111"
                        };

                        //time.Marke("9.开始插入失败日志", rid.ToString());
                        _exportErrorRepository.Insert(error);
                        //time.Marke("10.失败日志插入完成", rid.ToString());
                    }
                }
                catch (Exception ex)
                {
                    //加入错误队列
                    //_redisHelper.ListRightPush(SUCCESSACTIDQUEUEKEY, actId.ToString());
                    Console.WriteLine(rid + "该活动导入数据出错了,错误原因：" + ex.Message);
                    //加入日志
                    ExportError error = new ExportError()
                    {
                        RowId = rid.ToString(),
                        ErrorMsg = ex.Message,
                        OperateException = ex.ToString()

                    };
                    //time.Marke("11.开始插入异常日志", rid.ToString());
                    _exportErrorRepository.Insert(error);
                    //time.Marke("12.异常日志插入完成", rid.ToString());
                }
                //一个活动保存一次
                time.SaveData(rid);
            }

        }

        public void StartWorkFor(int id)
        {
            ExportAct(id);
        }

        #region  ExportData 导出活动数据


        /// <summary>
        /// 返回上次最后导入场景：删除导入错误时候的数据
        /// </summary>
        public void BackToLastExport()
        {
            //time.Marke("13.获取redisKey,返回导入场景");
            string actStr = _redisHelper.StringGet(SUCCESSACTIDQUEUEKEY);
            //time.Marke("14.获取redisKey完成,返回导入场景");
            if (!string.IsNullOrEmpty(actStr))
            {
                Console.WriteLine("停止前最后导入的数据id:" + actStr);

                //time.Marke("15.查询5条可能出错的异常信息 select top  5 *  from  SignPointActivity where id>@id");
                IEnumerable<SignPointActivityDto> actList = _signPointActivityRepository.Query<SignPointActivityDto>("select top  5 *  from  SignPointActivity where id>@id", new { id = Convert.ToInt32(actStr) });
                if (actList.Any())
                {
                    Console.WriteLine("sqlserver数据库记录停止导入的后续前5条数据：" + string.Join(",", actList.Select(t => t.Id)));
                    foreach (var act in actList)
                    {
                        //time.Marke("16.开始判断是否有脏数据", act.Id.ToString());
                        if (_signActRepositoryMongo.FirstOrDefault(act.ActID.ToString()) != null)
                        {
                            //time.Marke("17.脏数据判断完成", act.Id.ToString());
                            Console.WriteLine("检测到脏数据:" + act.Id);

                            //time.Marke("18.开始删除导出的错误数据", act.Id.ToString());
                            DeleteActData(act.Id, act.ActID.ToString());
                            //time.Marke("19.导出的错误数据完成", act.Id.ToString());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("数据已经全部倒完");
                    return;
                }
            }
            else
            {
                return;
            }

        }

        /// <summary>
        /// 删除导入错误的数据
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="actId"></param>
        public void DeleteActData(int rowId, string actId)
        {
            _signActRepositoryMongo.Delete(actId);

            //time.Marke("20.脏数据查询开始", actId);
            IEnumerable<string> openIdList = _signRecordsRepository
                .Query<string>("select distinct  openid  from   SignRecords where  ActivityID=@actID ",
                    new { actID = rowId });

            //time.Marke("21.脏数据查询结束", actId);
            foreach (var openId in openIdList)
            {
                //time.Marke("22.脏数据删除开始", actId);
                _userSignActRepositoryMongo.Delete(UserSignActivity.GetId(openId, actId));
                //time.Marke("23.脏数据删除结束", actId);

            }
        }

        /// <summary>
        /// 删除key
        /// </summary>
        public void DelectRedisCount()
        {
            _redisHelper.KeyDelete(SUCCESSACTIDQUEUEKEY);

        }


        /// <summary>
        ///导出活动以及活动设置数据
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        public bool ExportAct(int rid)
        {

            //time.Marke("24.执行查询活动的信息 select top 1 * from SignPointActivity where  ID=@id;select  * from   SignPointSet where  ActivityID=@id ", rid.ToString());
            List<IEnumerable<object>> queryList = _signPointActivityRepository.QueryMultiple(
                "select top 1 * from SignPointActivity where  ID=@id;select  * from   SignPointSet where  ActivityID=@id",
                new { id = rid }, typeof(SignPointActivityDto), typeof(SignPointSetDto));
            //time.Marke("25.结束查询活动的信息 select top 1 * from SignPointActivity where  ID=@id;select  * from   SignPointSet where  ActivityID=@id ", rid.ToString());

            SignPointActivityDto signAct = (SignPointActivityDto)queryList[0].FirstOrDefault();
            //IEnumerable<SignPointSetDto> actSetList =(IEnumerable<SignPointSetDto>) queryList[1];
            List<SignPointSetDto> actSetList = new List<SignPointSetDto>();

            //time.Marke("26.循环更改SignPointSetDto数据结构", rid.ToString());
            foreach (var item in queryList[1])
            {
                actSetList.Add((SignPointSetDto)item);
            }
            //time.Marke("27.循环更改SignPointSetDto数据结构结束", rid.ToString());


            //time.Marke("28.SignActivity map过程开始", rid.ToString());
            SignActivity signActEntity = signAct.MapTo<SignActivity>();
            //time.Marke("29.SignActivity map过程结束", rid.ToString());

            if (signAct != null)
            {
                signActEntity.Id = signAct.ActID.ToString();
                signActEntity.ActID = signActEntity.Id;
                signActEntity.RowId = rid;
                signActEntity.CreateTime = signAct.Addtime;
            }


            if (actSetList == null || !actSetList.Any())
            {

                //time.Marke("30.活动奖项不存在异常开始", rid.ToString());
                //throw new ArgumentException("活动奖项不存在。");
                //日志记录
                ExportError error = new ExportError()
                {

                    RowId = rid.ToString(),
                    ErrorMsg = rid + "活动没有奖项信息",
                    OperateException = ""

                };
                _exportErrorRepository.Insert(error);

                //time.Marke("31.活动奖项不存在异常结束", rid.ToString());
                return false;
            }

            List<SignPointSetEntity> setEntityList = new List<SignPointSetEntity>();

            //time.Marke("32.循环添加对象List<SignPointSetEntity>转换开始", rid.ToString());
            foreach (var set in actSetList)
            {
                var temp = set.MapTo<SignPointSetEntity>();
                temp.ActID = signActEntity.Id;
                setEntityList.Add(temp);
            }

            //time.Marke("33.循环添加对象List<SignPointSetEntity>转换结束", rid.ToString());

            signActEntity.SignPointSet = setEntityList;

            //time.Marke("34.判断插入的数据是否重复开始", rid.ToString());

            //插入数据到mongoDB
            if (_signActRepositoryMongo.FirstOrDefault(signActEntity.Id) == null)
            {
                //time.Marke("35.判断插入的数据是否重复结束", rid.ToString());

                //time.Marke("36.插入活动信息开始", rid.ToString());
                //插入活动
                _signActRepositoryMongo.Insert(signActEntity);
                //time.Marke("37.插入活动信息结束", rid.ToString());

                //time.Marke("38.填入库存缓存数据到redis开始", rid.ToString());
                //填入库存缓存数据到redis
                ExportCacheData(setEntityList, rid);
                //time.Marke("39.填入库存缓存数据到redis结束", rid.ToString());
                //time.Marke("40.插入用户签到，领奖记录开始", rid.ToString());
                //插入用户签到，领奖记录
                ExportUserSign(rid, signActEntity.Id);
                //time.Marke("41.插入用户签到，领奖记录结束", rid.ToString());
                return true;
            }
            else
            {
                Console.WriteLine(rid + "活动数据重复导入");
                //ExportError error = new ExportError()
                //{ rid = rid.ToString(), ErrorMsg = rid + "活动数据重复导入", OperateException = "" };
                //_exportErrorRepository.Insert(error);
                return true;
            }
            return true;
        }


        /// <summary>
        /// 导出进行中活动的库存数据到Redis
        /// </summary>
        /// <param name="setEntityList"></param>
        private void ExportCacheData(List<SignPointSetEntity> setEntityList, int originalId)
        {
            //Parallel.ForEach(setEntityList, item =>
            //{

            //    if (item.WxHbID > 0)//红包
            //    {
            //        int prizeType = 0;
            //        int activityType = item.IsSpecial == 0 ? 2 : 3;
            //        string hbStockCountKey = string.Format(SignKeys.signactprizegrantnum_key, item.ID, prizeType);
            //        hbStockCountKey = "B200016:" + hbStockCountKey;
            //        int hbGrantCount = _eventAwardRecordsRepository.Count(" where MemberLogin=@Memberlogin and ActivityID=@ActID  and ActivityType=@ActivityType and (PrizeType=@PrizeType or  PrizeType=@PrizeType2 )  and AwardSetId = @AwardSetId",
            //            new { Memberlogin = item.Memberlogin, ActID = originalId, ActivityType = activityType, PrizeType = 5, PrizeType2 = 6, AwardSetId = item.ID });
            //        bool flag = _redisHelper.StringSet(hbStockCountKey, hbGrantCount.ToString(), 180 * 60 * 60 * 24);
            //        //_redisHelper.StringIncrement(hbStockCountKey, hbGrantCount);

            //    }
            //    if (item.ProductID > 0)//产品
            //    {
            //        int prizeType = 1;
            //        int activityType = item.IsSpecial == 0 ? 2 : 3;
            //        string productStockCountKey = string.Format(SignKeys.signactprizegrantnum_key, item.ID, prizeType);
            //        productStockCountKey = "B200016:" + productStockCountKey;
            //        int prizeTypeDB = 0;
            //        int productStockCount = _eventAwardRecordsRepository.Count("where MemberLogin=@Memberlogin and ActivityID=@ActID  and ActivityType=@ActivityType and PrizeType=@PrizeType  and AwardSetId = @AwardSetId",
            //            new { Memberlogin = item.Memberlogin, ActID = originalId, ActivityType = activityType, PrizeType = prizeTypeDB, AwardSetId = item.ID });
            //        bool flag = _redisHelper.StringSet(productStockCountKey, productStockCount.ToString(), 180 * 60 * 60 * 24);
            //        //_redisHelper.StringIncrement(productStockCountKey, productStockCount);
            //    }

            //});

            foreach (var item in setEntityList)
            {
                if (item.WxHbID > 0)//红包
                {

                    //time.Marke("42.红包");
                    int prizeType = 0;
                    int activityType = item.IsSpecial == 0 ? 2 : 3;
                    string hbStockCountKey = string.Format(SignKeys.signactprizegrantnum_key, item.ID, prizeType);
                    hbStockCountKey = "B200016:" + hbStockCountKey;
                    int hbGrantCount = _eventAwardRecordsRepository.Count(" where MemberLogin=@Memberlogin and ActivityID=@ActID  and ActivityType=@ActivityType and (PrizeType=@PrizeType or  PrizeType=@PrizeType2 )  and AwardSetId = @AwardSetId",
                        new { Memberlogin = item.Memberlogin, ActID = originalId, ActivityType = activityType, PrizeType = 5, PrizeType2 = 6, AwardSetId = item.ID });
                    bool flag = _redisHelper.StringSet(hbStockCountKey, hbGrantCount.ToString(), 180 * 60 * 60 * 24);
                    //_redisHelper.StringIncrement(hbStockCountKey, hbGrantCount);

                    //time.Marke("43.红包结束");

                }
                if (item.ProductID > 0)//产品
                {
                    //time.Marke("44.产品");
                    int prizeType = 1;
                    int activityType = item.IsSpecial == 0 ? 2 : 3;
                    string productStockCountKey = string.Format(SignKeys.signactprizegrantnum_key, item.ID, prizeType);
                    productStockCountKey = "B200016:" + productStockCountKey;
                    int prizeTypeDB = 0;
                    int productStockCount = _eventAwardRecordsRepository.Count("where MemberLogin=@Memberlogin and ActivityID=@ActID  and ActivityType=@ActivityType and PrizeType=@PrizeType  and AwardSetId = @AwardSetId",
                        new { Memberlogin = item.Memberlogin, ActID = originalId, ActivityType = activityType, PrizeType = prizeTypeDB, AwardSetId = item.ID });
                    bool flag = _redisHelper.StringSet(productStockCountKey, productStockCount.ToString(), 180 * 60 * 60 * 24);
                    //_redisHelper.StringIncrement(productStockCountKey, productStockCount);

                    //time.Marke("45.产品");
                }
            }
        }


        /// <summary>
        /// 导出用户签到数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExportUserSign(int id, string guid)
        {
            //time.Marke("46.执行 select distinct  openid  from   SignRecords where  ActivityID=@actID ", id.ToString());
            IEnumerable<string> openIdList = _signRecordsRepository
                .Query<string>("select distinct  openid  from   SignRecords where  ActivityID=@actID ",
                    new { actID = id });
            //time.Marke("47.执行结束 select distinct  openid  from   SignRecords where  ActivityID=@actID ", id.ToString());

            if (openIdList == null || !openIdList.Any()) return true;//无人参加
            int index = 0;
            List<UserSignActivity> userSignList = new List<UserSignActivity>();

            //time.Marke("48.判断是否包含这条用户签到数据", id.ToString());
            if (_userSignActRepositoryMongo.FirstOrDefault(UserSignActivity.GetId(openIdList.FirstOrDefault(), guid)) == null)
            {
                //time.Marke("49.结束判断是否包含这条用户签到数据", id.ToString());
                int cindex = 0;
                foreach (var openid in openIdList)
                {
                    cindex++;
                    //time.Marke("50." + cindex + ".循环查询用户数据", id.ToString());
                    List<IEnumerable<object>> queryList = _signPointActivityRepository.QueryMultiple(
                        "select  *   from   SignRecords where  ActivityID=@actID and Openid=@openId order by ID desc; " +
                        "select  *   from   SignSpecialRecords where  ActivityID=@actID and Openid=@openId; " +
                        "select  *   from   MemberSignatureCard where  ActivityID=@actID and Openid=@openId; " +
                        //"select  *   from   EventAwardRecords where  ActivityID=@actID and Openid=@openId and (ActivityType=@actType or ActivityType=@actSpecialType ",
                        "select  *   from   EventAwardRecords where  ActivityID=@actID and Openid=@openId and ActivityType=@actType union  select  *   from   EventAwardRecords where  ActivityID=@actID and Openid=@openId and ActivityType=@actSpecialType ",
                        new { actID = id, openId = openid, actType = 2, actSpecialType = 3 }, new Type[] { typeof(SignRecordsDto), typeof(SignSpecialRecordsDto), typeof(MemberSignatureCardDto), typeof(EventAwardRecordsDto) });
                    //time.Marke("51." + cindex + ".循环查询用户数据", id.ToString());
                    //time.Marke("52." + cindex + ".数据赋值", id.ToString());
                    List<SignRecordsEntity> recordEntityList = GetUserRecords(queryList[0], guid);//时间先后倒叙排序，最新的在最前面 
                    List<SignSpecialRecordsEntity> specialRecordEntityList = GetUserSpecialRecords(queryList[1], guid);
                    List<MemberSignatureCardEntity> cardEntityList = GetUserCardList(queryList[2], guid, id);
                    List<EventAwardRecordsEntity> eventAwardEntityList = GetEventAwardList(guid, queryList[3]);
                    UserSignActivity userSign = new UserSignActivity(openid, guid)
                    {
                        LastSignTime = getLastSignTime(recordEntityList),
                        MaxTotalNumber = recordEntityList.Max(t => t.TotalNumber),
                        SignRecords = recordEntityList,
                        SignSpecialRecords = specialRecordEntityList,
                        MemberSignatureCard = cardEntityList,
                        EventAwardRecords = eventAwardEntityList
                    };
                    userSignList.Add(userSign);
                    //time.Marke("53." + cindex + ".数据赋值结束", id.ToString());
                    index++;
                    if (index >= 1000)
                    {
                        //time.Marke("54." + cindex + ".1000条插入mongodb开始", id.ToString());
                        _userSignActRepositoryMongo.InsertList(userSignList);
                        //time.Marke("55." + cindex + ".1000条插入mongodb结束", id.ToString());
                        index = 0;
                        //time.Marke("56." + cindex + ".清理列表开始", id.ToString());
                        userSignList.Clear();
                        //time.Marke("57." + cindex + ".清理列表结束", id.ToString());
                    }
                }
                //time.Marke("58.剩余的数据插入mongodb开始." + userSignList.Count, id.ToString());
                _userSignActRepositoryMongo.InsertList(userSignList);
                //time.Marke("59.剩余的数据插入mongodb结束." + userSignList.Count, id.ToString());
                return true;
            }
            else
            {
                return false;
            }


        }



        #endregion



        #region MyRegion  辅助



        /// <summary>
        /// 获取最近一次的签到时间
        /// </summary>
        /// <param name="recordEntityList"></param>
        /// <returns></returns>
        private DateTime getLastSignTime(List<SignRecordsEntity> recordEntityList)
        {
            if (!recordEntityList.Any()) return DateTime.Now.AddDays(-2);//不存在
            DateTime maxSignTime = DateTime.Now.Date;
            DateTime cardSignTime = DateTime.Now.Date;
            int count = 0;
            DateTime signTime = Convert.ToDateTime(recordEntityList.FirstOrDefault().YYYYMMDD);
            foreach (var temp in recordEntityList)
            {
                if (temp.IsSignature == 0)
                {
                    if (maxSignTime.Date == DateTime.Now.Date) maxSignTime = Convert.ToDateTime(temp.YYYYMMDD);
                }
                else
                {
                    DateTime tempTime = Convert.ToDateTime(temp.YYYYMMDD);
                    if (cardSignTime == DateTime.Now.Date) signTime = tempTime;
                    else
                    {
                        if (Convert.ToDateTime(temp.YYYYMMDD) > cardSignTime)
                            cardSignTime = Convert.ToDateTime(temp.YYYYMMDD);
                    }
                }
            }
            if (maxSignTime.Date == DateTime.Now.Date && cardSignTime.Date == DateTime.Now.Date) return DateTime.Now.AddDays(-2).Date;//不存在
            else if (maxSignTime.Date != DateTime.Now.Date && cardSignTime.Date != DateTime.Now.Date) return maxSignTime > cardSignTime ? maxSignTime : cardSignTime;
            else if (maxSignTime.Date != DateTime.Now.Date) return maxSignTime;
            else return cardSignTime;
        }

        private List<EventAwardRecordsEntity> GetEventAwardList(string actId, IEnumerable<object> awardList)
        {
            //IEnumerable<EventAwardRecordsDto> awardList = _eventAwardRecordsRepository.Query<EventAwardRecordsDto>
            //    ("select  *   from   EventAwardRecords where  ActivityID=@actID and Openid=@openId and (ActivityType=@actType or ActivityType=@actSpecialType) ",
            //    new { actID = id, openId = openid, actType = 2, actSpecialType = 3 });
            List<EventAwardRecordsEntity> awardEntityList = new List<EventAwardRecordsEntity>();
            if (awardList != null && awardList.Any())
            {
                foreach (var card in awardList)
                {
                    var temp = card.MapTo<EventAwardRecordsEntity>();
                    temp.ActID = actId;
                    awardEntityList.Add(temp);
                }
            }
            return awardEntityList;
        }

        private List<MemberSignatureCardEntity> GetUserCardList(IEnumerable<object> cardList, string guid, int Id)
        {
            //IEnumerable<MemberSignatureCardDto> cardList = _memberSignatureCardRepository.Query<MemberSignatureCardDto>("select  *   from   MemberSignatureCard where  ActivityID=@actID and Openid=@openId ", new { actID = id, openId = openid });
            List<MemberSignatureCardEntity> cardEntityList = new List<MemberSignatureCardEntity>();
            if (cardList != null && cardList.Any())
            {
                foreach (var card in cardList)
                {
                    var temp = card.MapTo<MemberSignatureCardEntity>();
                    temp.ActivityID = guid;
                    cardEntityList.Add(temp);
                }
            }
            //更新补签卡统计
            //fact_Statistic_SignatureCard cardStatic = _signCardStaticRepository.Query<fact_Statistic_SignatureCard>(
            //    "select top 1 * from fact_Statistic_SignatureCard where  ActivityID=@id",
            //    new { id = rid }).FirstOrDefault();
            //if (cardStatic != null)
            //{
            //    cardStatic.ActID = guid;
            //    cardStatic.rid = cardStatic.ID;
            //    _signCardStaticRepository.Update(cardStatic);
            //}
            return cardEntityList;
        }

        private List<SignSpecialRecordsEntity> GetUserSpecialRecords(IEnumerable<object> signRecords, string guid)
        {
            //IEnumerable<SignSpecialRecordsDto> signRecords = _signSpecialRecordsRepository.Query<SignSpecialRecordsDto>("select  *   from   SignSpecialRecords where  ActivityID=@actID and Openid=@openId ", new { actID = id, openId = openid });
            List<SignSpecialRecordsEntity> recordEntityList = new List<SignSpecialRecordsEntity>();
            if (signRecords != null && signRecords.Any())
            {
                foreach (var signRecord in signRecords)
                {
                    var temp = signRecord.MapTo<SignSpecialRecordsEntity>();
                    temp.ActivityID = guid;
                    recordEntityList.Add(temp);
                }
            }
            return recordEntityList;
        }

        private List<SignRecordsEntity> GetUserRecords(IEnumerable<object> signRecords, string guid)
        {
            //IEnumerable<SignRecordsDto> signRecords = _signRecordsRepository.Query<SignRecordsDto>("select  *   from   SignRecords where  ActivityID=@actID and Openid=@openId order by ID desc ", new { actID = id, openId = openid });
            List<SignRecordsEntity> recordEntityList = new List<SignRecordsEntity>();
            if (signRecords != null && signRecords.Any())
            {
                foreach (SignRecordsDto signRecord in signRecords)
                {
                    if (signRecord.YYYYMMDD.IndexOf("-") != -1)
                    {
                        var temp = signRecord.MapTo<SignRecordsEntity>();
                        temp.ActivityID = guid;
                        if (temp.SpecialID == "0") temp.SpecialID = null;
                        recordEntityList.Add(temp);
                    }
                    else if (!string.IsNullOrEmpty(signRecord.YYYYMMDD))
                    {
                        var mapper = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<SignRecordsDto, SignRecordsEntity>()
                           .ForMember(dest => dest.YYYYMMDD, opt => opt.MapFrom(src => DateTime.ParseExact(src.YYYYMMDD, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture)));
                        }).CreateMapper();
                        var temp = mapper.Map<SignRecordsEntity>(signRecord);
                        temp.ActivityID = guid;
                        if (temp.SpecialID == "0") temp.SpecialID = null;
                        recordEntityList.Add(temp);
                    }
                    else
                    {
                        var mapper = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<SignRecordsDto, SignRecordsEntity>()
                                .ForMember(dest => dest.YYYYMMDD, opt => opt.MapFrom(src => src.SignTime));
                        }).CreateMapper();
                        var temp = mapper.Map<SignRecordsEntity>(signRecord);
                        temp.ActivityID = guid;
                        if (temp.SpecialID == "0") temp.SpecialID = null;
                        recordEntityList.Add(temp);
                    }

                }
            }
            return recordEntityList;
        }

        #endregion



    }
}