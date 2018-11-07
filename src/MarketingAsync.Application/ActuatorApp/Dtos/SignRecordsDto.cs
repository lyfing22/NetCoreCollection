/*----------------------------------------------------------------
      // Copyright (C) 2017 广东米多网络科技有限公司-营销活动
      // 创建时间：2018-03-02
      // 创建人：  刘卫国
      // 功能描述：签到记录表 SignRecords 模型类
----------------------------------------------------------------*/

using System;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.Act.User;

namespace MarketingAsync.ActuatorApp.Dtos
{
    /// <summary>
    /// 签到记录表SignRecords
    /// </summary>
    [AutoMap(typeof(SignRecordsEntity))]
    public class SignRecordsDto : Entity
    {
        /// <summary>
        /// ID，主键，自增
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        ///签到会员账号
        /// </summary>
        public string Openid { get; set; }

        /// <summary>
        ///会员昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        ///签到日期
        /// </summary>
        public DateTime SignTime { get; set; }


        /// <summary>
        /// 老 签到日期
        /// </summary>
        public string YYYYMMDD { get; set; }

        /// <summary>
        ///连续签到天数
        /// </summary>
        public int TotalNumber { get; set; }

        /// <summary>
        ///是否是继续签到
        /// </summary>

        public int IsContinue { get; set; }

        /// <summary>
        ///签到获得积分
        /// </summary>

        public int Point { get; set; }

        /// <summary>
        ///获得金币
        /// </summary>

        public decimal Amount { get; set; }

        /// <summary>
        ///总购物券面值
        /// </summary>

        public decimal ShopVouAmount { get; set; }

        /// <summary>
        ///购物券ID，对应购物券设置表ShopVoucherSetting中字段ID，非聚集索引列，获取多张购物券用，隔开。
        /// </summary>

        public string ShopVouID { get; set; }

        /// <summary>
        ///红包金额
        /// </summary>

        public decimal hbAmount { get; set; }

        /// <summary>
        ///红包备注
        /// </summary>

        public string hbRemark { get; set; }

        /// <summary>
        ///红包发放状态：0：未发放；1：发放成功；-1：发放失败
        /// </summary>

        public int hbSend { get; set; }

        /// <summary>
        ///商家帐号，对应品牌商账号登记表Account中字段MemberLogin。
        /// </summary>

        public string memberlogin { get; set; }

        /// <summary>
        ///记录时间
        /// </summary>

        public DateTime Addtime { get; set; }

        /// <summary>
        ///签到排名
        /// </summary>

        public int Ranking { get; set; }

        /// <summary>
        ///签到活动ID，对应签到活动表SignPointActivity中字段ID，非聚集索引列。
        /// </summary>

        public int ActivityID { get; set; }

        /// <summary>
        ///对应签到活动设置表 SignPointSet中字段Day，非聚集索引列。
        /// </summary>

        public int Day { get; set; }

        /// <summary>
        ///红包数量是否发放完0：默认有剩余；1：发放完
        /// </summary>

        public int hbNumberNull { get; set; }

        /// <summary>
        ///购物券发放状态1：发放成功；2：发放失败
        /// </summary>

        public int SvSend { get; set; }

        /// <summary>
        ///购物券发放失败备注
        /// </summary>

        public string SvRemark { get; set; }

        /// <summary>
        ///红包ID，关联红包日志表HbLogs中字段ID
        /// </summary>

        public int HbLogId { get; set; }

        /// <summary>
        ///邀请人Openid
        /// </summary>

        public string Inviter { get; set; }

        ///// <summary>
        /////时间戳
        ///// </summary>
        //
        //public Byte LastTimestamp { get; set; }

        /// <summary>
        ///产品ID，对应商城商品登记表Products中字段ID，非聚集索引列。
        /// </summary>

        public int ProductID { get; set; }

        /// <summary>
        ///产品数量是否发放完0：默认有剩余；1：发放完
        /// </summary>

        public int ProNumNull { get; set; }

        /// <summary>
        ///是否补签记录0：否；1：是
        /// </summary>

        public int IsSignature { get; set; }

        /// <summary>
        ///会员补签卡ID，对应会员补签卡表MemberSignatureCard中字段ID。
        /// </summary>

        public int SignatureCardID { get; set; }

        /// <summary>
        ///特殊规则记录表ID，对应签到特殊规则记录表SignSpecialRecords中字段ID，非聚集索引列。
        /// </summary>

        public int SpecialID { get; set; }

        /// <summary>
        ///是否已发放：0：未发放（未领取）；1：已发放(已领取)2：领取失败
        /// </summary>

        public int IsSend { get; set; }

        /// <summary>
        ///订单ID，对应订单登记表Orders中字段OrderID，非聚集索引列。
        /// </summary>

        public int OrderID { get; set; }

        /// <summary>
        ///用户ID Customer表ID
        /// </summary>

        public int CustomerID { get; set; }

        /// <summary>
        ///用户头像Customer表HeadPath
        /// </summary>

        public string CustomerHeadPath { get; set; }

        /// <summary>
        ///红包类型0：微信红包；1：零钱红包
        /// </summary>


        public int WxHbType { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string UserIp { get; set; }



    }

}
