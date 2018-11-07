/*----------------------------------------------------------------
      // Copyright (C) 2017 广东米多网络科技有限公司-营销活动
      // 创建时间：2018-03-02
      // 创建人：  刘卫国
      // 功能描述：签到活动设置表 SignPointSet 模型类
----------------------------------------------------------------*/

using System;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using MarketingAsync.Act.Act;
using MarketingAsync.Act.Sqlserver;

namespace MarketingAsync.ActuatorApp.Dtos
{
    /// <summary>
    /// 签到活动设置表SignPointSet
    /// </summary>

    [AutoMap(typeof(SignPointSetEntity))]
    public class SignPointSetDto : Entity
    {
        /// <summary>
        /// ID，主键，自增
        /// </summary>
        public new string Id { get; set; }

        /// <summary>
        ///签到活动ID，对应签到活动表SignPointActivity中字段ID，非聚集索引列。
        /// </summary>

        public int ActivityID { get; set; }

        /// <summary>
        ///连续天数
        /// </summary>

        public int Day { get; set; }

        /// <summary>
        ///获得积分
        /// </summary>

        public int Point { get; set; }

        /// <summary>
        ///获得金币
        /// </summary>

        public decimal Amount { get; set; }

        /// <summary>
        ///购物券ID，对应购物券设置表ShopVoucherSetting中字段ID，非聚集索引列。
        /// </summary>

        public int ShopVouID { get; set; }

        /// <summary>
        ///微信红包ID   关联WxHongBaoSetting 表ID
        /// </summary>

        public int WxHbID { get; set; }

        /// <summary>
        ///类型：0：固定红包；1：随机红包2：无红包（之前为作废字段，判断时候需要注意老数据）
        /// </summary>

        public int hbType { get; set; }

        /// <summary>
        ///最小红包金额（固定红包最大最小金额一样）
        /// </summary>

        public decimal MinMoney { get; set; }

        /// <summary>
        ///最大红包金额（固定红包最大最小金额一样）
        /// </summary>

        public decimal MaxMoney { get; set; }

        /// <summary>
        ///商家帐号，对应品牌商账号登记表Account中字段MemberLogin。
        /// </summary>

        public string Memberlogin { get; set; }

        /// <summary>
        ///添加时间
        /// </summary>

        public DateTime Addtime { get; set; }

        /// <summary>
        ///红包奖品数量
        /// </summary>

        public int hbNumber { get; set; }

        /// <summary>
        ///产品ID，对应商城商品登记表Products中字段ID。
        /// </summary>

        public int ProductID { get; set; }

        /// <summary>
        ///产品数量
        /// </summary>

        public int ProductNum { get; set; }

        /// <summary>
        ///是否是特殊规则0：否；1：是
        /// </summary>

        public int IsSpecial { get; set; }

        /// <summary>
        ///签到日期
        /// </summary>

        public string SpecialDate { get; set; }

        /// <summary>
        ///是否删除0：否；1：是
        /// </summary>

        public int IsDelete { get; set; }

        ///// <summary>
        /////
        ///// </summary>
        //
        //public int HBReceiveNum { get; set; }

        ///// <summary>
        /////
        ///// </summary>
        //
        //public int ProductReceiveNum { get; set; }

        /// <summary>
        ///红包类型0：微信红包；1：零钱红包
        /// </summary>

        public int WxHbType { get; set; }

        /// <summary>
        ///运费模板id
        /// </summary>

        public int FreightId { get; set; }

        /// <summary>
        /// 是否显示红包金额 0不显示  1显示
        /// </summary>

        public int ShowHbSum { get; set; }



    }
}
