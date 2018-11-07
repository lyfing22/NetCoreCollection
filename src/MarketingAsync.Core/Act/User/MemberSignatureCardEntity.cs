/*----------------------------------------------------------------
      // Copyright (C) 2017 广东米多网络科技有限公司-营销活动
      // 创建时间：2018-03-13
      // 创建人：  刘卫国
      // 功能描述：会员补签卡表 MemberSignatureCard 模型类
----------------------------------------------------------------*/

using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MarketingAsync.Act.User
{
    /// <summary>
    /// 会员补签卡
    /// </summary>
    public class MemberSignatureCardEntity
    {
        /// <summary>
        /// ID, 生成的guid
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        ///商家账号
        /// </summary>
        public string Memberlogin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OpenID { get; set; }

        /// <summary>
        /// 签到活动ID，对应签到活动表SignPointActivity中字段ID
        /// </summary>
        public string ActivityID { get; set; }

        /// <summary>
        /// 补签卡来源0：积分兑换补签卡
        /// </summary>
        public int Source { get; set; }

        /// <summary>
        /// 是否已使用0：未使用；1：已使用
        /// </summary>
        public int IsUse { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 会员ID
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 会员昵称
        /// </summary>
        public string CustomerNickName { get; set; }
        /// <summary>
        /// 会员头像
        /// </summary>
        public string CustomerHeadPath { get; set; }
    }
}
