/*----------------------------------------------------------------
      // Copyright (C) 2017 广东米多网络科技有限公司-营销活动
      // 创建时间：2017-12-20
      // 创建人：  邹晓龙
      // 功能描述：营销活动奖品记录表
----------------------------------------------------------------*/

using System;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.Act.User;

namespace MarketingAsync.ActuatorApp.Dtos
{
    /// <summary>
    /// 营销活动奖品记录表
    /// </summary>

    [AutoMap(typeof(EventAwardRecordsEntity))]
    public class EventAwardRecordsDto : Entity
    {
        /// <summary>
        /// 订单ID，主键，自增
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>

        public int ActivityID { get; set; }

        /// <summary>
        /// 记录ID  如 抽奖记录 签到记录 , activityPrize奖项表ID
        /// </summary>

        public int RecordsID { get; set; }

        /// <summary>
        /// OpenID
        /// </summary>

        public string OpenID { get; set; }

        /// <summary>
        /// 商家账号
        /// </summary>

        public string MemberLogin { get; set; }

        /// <summary>
        /// 活动类型：
        ///0：大转盘抽奖记录
        ///1：刮刮卡抽奖记录
        ///2：签到
        ///3：签到特殊规则
        ///4:问卷调查
        ///5：投票评选
        ///6：砍价d
        ///7：梦想助力
        ///8：微现场
        ///9：摇一摇送红包
        ///10：口令红包
        ///11:集字有礼抽奖记录
        ///12：营销活动
        ///13：竞猜
        ///14：消消乐
        /// </summary>

        public int ActivityType { get; set; }

        /// <summary>
        /// 奖品类型
        ///0:产品
        ///1：积分
        ///2：购物券
        ///3：第三方卡券
        ///4：金币
        ///5：微信红
        ///6：红包零钱
        /// </summary>

        public int PrizeType { get; set; }

        /// <summary>
        /// 奖项名称 （如一等奖 二等奖之类的）
        /// </summary>

        public string PrizeName { get; set; }

        /// <summary>
        /// 奖品名称
        /// </summary>

        public string ProductName { get; set; }

        /// <summary>
        /// 奖品ID（兼容以前签到发购物券的数据，数据类型保存为nvarchar类型）
        ///  PrizeType = 0 products 表ID
        ///    PrizeType = 1 为空
        ///    PrizeType = 2 购物券设置表ID
        ///   PrizeType = 3 和 PrizeType = 4 WxHongBaoSetting 表ID
        /// </summary>

        public string PrizeID { get; set; }

        /// <summary>
        /// PrizeType = 1 积分值
        ///  PrizeType = 2  购物券面值
        ///  PrizeType = 3 和 PrizeType = 4 红包金额
        /// </summary>

        public decimal Amount { get; set; }

        /// <summary>
        /// 是否已发放：
        ///-1 发放失败
        ///0：未发放（未领取）
        ///1：已发放(已领取)
        ///2：补发成功
        ///3：发放中（联盟卡券,微信红包）
        ///4：补发中
        /// </summary>

        public int IsSend { get; set; }


        /// <summary>
        /// 发放失败原因
        /// </summary>

        public string FailRemark { get; set; }
        /// <summary>
        /// PrizeType = 0 订单ID
        /// PrizeType = 3 hbLogs ID
        /// PrizeType = 4  pmlogs ID
        /// PrizeType = 2：购物券发放ID
        /// </summary>

        public int LogsID { get; set; }


        public int SpecialID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>

        public string Remark { get; set; }

        /// <summary>
        /// 中奖时间
        /// </summary>

        public DateTime WinningTime { get; set; }

        /// <summary>
        /// 时间
        /// </summary>

        public DateTime AddTime { get; set; }

        /// <summary>
        /// 联盟卡劵订单号 / 卡包订单号
        /// </summary>

        public string OrderNum { get; set; }

        /// <summary>
        /// 红包发放接口返回订单号
        /// </summary>

        public string TradeNo { get; set; }

        /// <summary>
        /// 运费模板Id 0：免运费
        /// </summary>

        public int FreightId { get; set; }

        /// <summary>
        /// 联盟卡劵回调加入卡包步骤失败 0:成功,1:失败
        /// </summary>

        public int AddCardState { get; set; }

        /// <summary>
        /// 奖品设置表id
        /// </summary>

        public string AwardSetId { get; set; }


    }
}
