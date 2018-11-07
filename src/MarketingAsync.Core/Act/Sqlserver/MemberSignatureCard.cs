/*----------------------------------------------------------------
      // Copyright (C) 2017 广东米多网络科技有限公司-营销活动
      // 创建时间：2018-03-13
      // 创建人：  刘卫国
      // 功能描述：会员补签卡表 MemberSignatureCard 模型类
----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace MarketingAsync.Act.Sqlserver
{
    /// <summary>
    /// 会员补签卡表MemberSignatureCard
    /// </summary>
    public class MemberSignatureCard : Entity
    {
        /// <summary>
        /// ID，主键，自增
        /// </summary>
        public new int Id { get; set; }

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

        public int ActivityID { get; set; }

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

        public DateTime AddTime { get; set; }

        /// <summary>
        /// 会员ID
        /// </summary>

        public int CustomerID { get; set; }

        /// <summary>
        /// 会员昵称
        /// </summary>

        public string CustomerNickName { get; set; }
    }
}
