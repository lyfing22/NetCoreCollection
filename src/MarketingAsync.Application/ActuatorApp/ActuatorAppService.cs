using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AutoMapper;
using MarketingAsync.Act.Act;
using MarketingAsync.Act.Keys;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.Act.User;
using MarketingAsync.ActuatorApp.Dtos;
using MarketingAsync.Dapper;
using MarketingAsync.Redis;

namespace MarketingAsync.ActuatorApp
{
    [AutoMap()]
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
        ISignPointActivityRepository signPointActivityRepository, MergeSignActivityDomainService mergeSignActivity, ISignActivityRepository signActRepositoryMongo, IUserSignActivityRepository userSignActRepositoryMongo, IRedisHelper redisHelper, ISignCardStaticRepository signCardStaticRepository, IExportErrorRepository exportErrorRepository)
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

        public static Stopwatch watch = new Stopwatch();

        public void SetStatisticalInformation(string index, State state)
        {
            return;
        }

        public void StartWork()
        {
            //ExportAct(559);
            int lastExportID = 0;
            string actStr = _redisHelper.StringGet(SUCCESSACTIDQUEUEKEY);
            if (!string.IsNullOrEmpty(actStr))
            {
                lastExportID = Convert.ToInt32(actStr);
            }

            IEnumerable<int> actIdList = _signPointActivityRepository.Query<int>("select distinct  id  from  SignPointActivity where id>@Id", new { Id = lastExportID });

            //执行任务
            ExcuteWorker(actIdList);

            Console.WriteLine("运行完毕, 请清理活动表（SignActivity）下的RowId 。。。");

        }

        public void ExcuteWorker(IEnumerable<int> actIdList)
        {
            //返回现场
            BackToLastExport();
            foreach (var actId in actIdList)
            {
                try
                {
                    if (SignalConfig.IsStop)
                    {
                        Console.WriteLine("收到停止导入命令，当前导入id(当前id未导入):" + actId);
                        return;
                    }

                    //导入活动数据+用户数据
                    if (ExportAct(actId))
                    {
                        _redisHelper.StringSet(SUCCESSACTIDQUEUEKEY, actId.ToString(), 60 * 60 * 24 * 3);
                    }
                    else
                    {
                        Console.WriteLine(actId + "该活动导入数据失败了");
                        //加入日志
                        ExportError error = new ExportError()
                        {
                            RowId = actId.ToString()
                            ,
                            ErrorMsg = "该活动导入数据失败了111"
                        };
                        _exportErrorRepository.Insert(error);
                    }
                }
                catch (Exception ex)
                {
                    //加入错误队列
                    //_redisHelper.ListRightPush(SUCCESSACTIDQUEUEKEY, actId.ToString());
                    Console.WriteLine(actId + "该活动导入数据出错了,错误原因：" + ex.Message);
                    //加入日志
                    ExportError error = new ExportError()
                    {
                        RowId = actId.ToString(),
                        ErrorMsg = ex.Message,
                        OperateException = ex.ToString()

                    };
                    _exportErrorRepository.Insert(error);
                }
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
            string actStr = _redisHelper.StringGet(SUCCESSACTIDQUEUEKEY);
            if (!string.IsNullOrEmpty(actStr))
            {
                Console.WriteLine("停止前最后导入的数据id:" + actStr);
                IEnumerable<SignPointActivityDto> actList = _signPointActivityRepository.Query<SignPointActivityDto>("select top  5 *  from  SignPointActivity where id>@id", new { id = Convert.ToInt32(actStr) });
                if (actList.Any())
                {
                    Console.WriteLine("sqlserver数据库记录停止导入的后续前5条数据：" + string.Join(",", actList.Select(t => t.Id)));
                    foreach (var act in actList)
                    {
                        if (_signActRepositoryMongo.FirstOrDefault(act.ActID.ToString()) != null)
                        {
                            Console.WriteLine("检测到脏数据:" + act.Id);
                            DeleteActData(act.Id, act.ActID.ToString());
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
            IEnumerable<string> openIdList = _signRecordsRepository
                .Query<string>("select distinct  openid  from   SignRecords where  ActivityID=@actID ",
                    new { actID = rowId });
            foreach (var openId in openIdList)
            {
                _userSignActRepositoryMongo.Delete(UserSignActivity.GetId(openId, actId));

            }
        }


        /// <summary>
        ///导出活动以及活动设置数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool ExportAct(int Id)
        {
            List<IEnumerable<object>> queryList = _signPointActivityRepository.QueryMultiple(
                "select top 1 * from SignPointActivity where  ID=@id;select  * from   SignPointSet where  ActivityID=@id",
                new { id = Id }, typeof(SignPointActivityDto), typeof(SignPointSetDto));
            SignPointActivityDto signAct = (SignPointActivityDto)queryList[0].FirstOrDefault();
            //IEnumerable<SignPointSetDto> actSetList =(IEnumerable<SignPointSetDto>) queryList[1];
            List<SignPointSetDto> actSetList = new List<SignPointSetDto>();
            foreach (var item in queryList[1])
            {
                actSetList.Add((SignPointSetDto)item);
            }
            SignActivity signActEntity = signAct.MapTo<SignActivity>();
            if (signAct != null)
            {
                signActEntity.Id = signAct.ActID.ToString();
                signActEntity.ActID = signActEntity.Id;
                signActEntity.RowId = Id;
                signActEntity.CreateTime = signAct.Addtime;
            }

            if (actSetList == null || !actSetList.Any())
            {
                //throw new ArgumentException("活动奖项不存在。");
                //日志记录
                ExportError error = new ExportError()
                { Id = Id.ToString(), ErrorMsg = Id + "活动没有奖项信息", OperateException = "" };
                _exportErrorRepository.Insert(error);
                return false;
            }
            List<SignPointSetEntity> setEntityList = new List<SignPointSetEntity>();
            foreach (var set in actSetList)
            {
                var temp = set.MapTo<SignPointSetEntity>();
                temp.ActID = signActEntity.Id;
                setEntityList.Add(temp);
            }

            signActEntity.SignPointSet = setEntityList;
            //插入数据到mongoDB
            if (_signActRepositoryMongo.FirstOrDefault(signActEntity.Id) == null)
            {
                //插入活动
                _signActRepositoryMongo.Insert(signActEntity);
                //填入库存缓存数据到redis
                ExportCacheData(setEntityList, Id);
                //插入用户签到，领奖记录
                ExportUserSign(Id, signActEntity.Id);
                return true;
            }
            else
            {
                Console.WriteLine(Id + "活动数据重复导入");
                //ExportError error = new ExportError()
                //{ Id = Id.ToString(), ErrorMsg = Id + "活动数据重复导入", OperateException = "" };
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
                    int prizeType = 0;
                    int activityType = item.IsSpecial == 0 ? 2 : 3;
                    string hbStockCountKey = string.Format(SignKeys.signactprizegrantnum_key, item.ID, prizeType);
                    hbStockCountKey = "B200016:" + hbStockCountKey;
                    int hbGrantCount = _eventAwardRecordsRepository.Count(" where MemberLogin=@Memberlogin and ActivityID=@ActID  and ActivityType=@ActivityType and (PrizeType=@PrizeType or  PrizeType=@PrizeType2 )  and AwardSetId = @AwardSetId",
                        new { Memberlogin = item.Memberlogin, ActID = originalId, ActivityType = activityType, PrizeType = 5, PrizeType2 = 6, AwardSetId = item.ID });
                    bool flag = _redisHelper.StringSet(hbStockCountKey, hbGrantCount.ToString(), 180 * 60 * 60 * 24);
                    //_redisHelper.StringIncrement(hbStockCountKey, hbGrantCount);

                }
                if (item.ProductID > 0)//产品
                {
                    int prizeType = 1;
                    int activityType = item.IsSpecial == 0 ? 2 : 3;
                    string productStockCountKey = string.Format(SignKeys.signactprizegrantnum_key, item.ID, prizeType);
                    productStockCountKey = "B200016:" + productStockCountKey;
                    int prizeTypeDB = 0;
                    int productStockCount = _eventAwardRecordsRepository.Count("where MemberLogin=@Memberlogin and ActivityID=@ActID  and ActivityType=@ActivityType and PrizeType=@PrizeType  and AwardSetId = @AwardSetId",
                        new { Memberlogin = item.Memberlogin, ActID = originalId, ActivityType = activityType, PrizeType = prizeTypeDB, AwardSetId = item.ID });
                    bool flag = _redisHelper.StringSet(productStockCountKey, productStockCount.ToString(), 180 * 60 * 60 * 24);
                    //_redisHelper.StringIncrement(productStockCountKey, productStockCount);
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
            IEnumerable<string> openIdList = _signRecordsRepository
                .Query<string>("select distinct  openid  from   SignRecords where  ActivityID=@actID ",
                    new { actID = id });
            if (openIdList == null || !openIdList.Any()) return true;//无人参加
            int index = 0;
            List<UserSignActivity> userSignList = new List<UserSignActivity>();
            if (_userSignActRepositoryMongo.FirstOrDefault(UserSignActivity.GetId(openIdList.FirstOrDefault(), guid)) == null)
            {
                foreach (var openid in openIdList)
                {
                    List<IEnumerable<object>> queryList = _signPointActivityRepository.QueryMultiple(
                        "select  *   from   SignRecords where  ActivityID=@actID and Openid=@openId order by ID desc; " +
                        "select  *   from   SignSpecialRecords where  ActivityID=@actID and Openid=@openId; " +
                        "select  *   from   MemberSignatureCard where  ActivityID=@actID and Openid=@openId; " +
                        //"select  *   from   EventAwardRecords where  ActivityID=@actID and Openid=@openId and (ActivityType=@actType or ActivityType=@actSpecialType ",
                        "select  *   from   EventAwardRecords where  ActivityID=@actID and Openid=@openId and ActivityType=@actType union  select  *   from   EventAwardRecords where  ActivityID=@actID and Openid=@openId and ActivityType=@actSpecialType ",
                        new { actID = id, openId = openid, actType = 2, actSpecialType = 3 }, new Type[] { typeof(SignRecordsDto), typeof(SignSpecialRecordsDto), typeof(MemberSignatureCardDto), typeof(EventAwardRecordsDto) });
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
                    index++;
                    if (index >= 1000)
                    {
                        _userSignActRepositoryMongo.InsertList(userSignList);
                        index = 0;
                        userSignList.Clear();
                    }
                }
                _userSignActRepositoryMongo.InsertList(userSignList);
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
            //    new { id = Id }).FirstOrDefault();
            //if (cardStatic != null)
            //{
            //    cardStatic.ActID = guid;
            //    cardStatic.Id = cardStatic.ID;
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
                        recordEntityList.Add(temp);
                    }

                }
            }
            return recordEntityList;
        }

        #endregion



    }
}