using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace MarketingAsync.Act.Act
{
    /// <summary>
    /// 签到活动表
    /// </summary>
    public class SignActivity : Entity<string>
    {
        public int RowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SignActivity()
        {
            SignPointSet = new List<SignPointSetEntity>();
            CreateTime = DateTime.Now;
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 签到活动设置表
        /// </summary>
        public IEnumerable<SignPointSetEntity> SignPointSet { get; set; }

        ///// <summary>
        ///// ID，主键，自增 Rid
        ///// </summary>
        //[Obsolete("准备弃用")]
        //public int ID { get; set; }

        /// <summary>
        /// 拓展id
        /// </summary>
        public string ActID { get { return Id; } set { Id = value; } }

        /// <summary>
        ///活动名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 签到备注
        /// </summary>
        public string RegistrationRemark { get; set; }

        /// <summary>
        /// 签到分享标题
        /// </summary>
        public string ShareTitle { get; set; }

        /// <summary>
        /// 签到分享图标
        /// </summary>
        public string ShareIco { get; set; }

        /// <summary>
        /// 商家帐号，对应品牌商账号登记表Account中字段MemberLogin。
        /// </summary>
        public string Memberlogin { get; set; }

        /// <summary>
        /// 启用状态：0：未启用；1：启用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Addtime { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //
        //public int TotalCount { get; set; }

        /// <summary>
        /// 签到开始时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 签到结束时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否删除  0：否；1：是
        /// </summary>
        public int IsDelete { get; set; }

        /// <summary>
        /// 模板ID，Shop_TempCustom  描述：微商城首页模板自定义表
        /// </summary>
        public int TempCustomID { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //
        //public string QrCodePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime QrCodeEndTime { get; set; }

        /// <summary>
        /// 背景图
        /// </summary>
        public string BackGroundImg { get; set; }

        /// <summary>
        /// 是否显示签到总数0：显示；1：隐藏
        /// </summary>
        public int IsShowANum { get; set; }

        /// <summary>
        /// 是否显示活动时间0：显示；1：隐藏
        /// </summary>
        public int IsShowATime { get; set; }

        /// <summary>
        /// 主题颜色
        /// </summary>
        public string ThemeColor { get; set; }

        /// <summary>
        /// 按钮设置的名称
        /// </summary>
        public string BtnName { get; set; }

        /// <summary>
        /// 按钮设置的链接名称
        /// </summary>
        public string BtnUrlName { get; set; }

        /// <summary>
        /// 按钮设置的链接
        /// </summary>
        public string BtnUrl { get; set; }

        /// <summary>
        /// 签到入口样式设置0：模板一1：模板二2：模板三
        /// </summary>
        public int CssStyle { get; set; }

        /// <summary>
        /// 是否开启补签功能0：关闭；1：开启
        /// </summary>
        public int IsSignature { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //
        //public int SignatureNum { get; set; }

        /// <summary>
        /// 兑换补签卡所需积分
        /// </summary>
        public int NeedPoint { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //
        //public Byte LastTimestamp { get; set; }

        /// <summary>
        /// 否开启额外奖励 0-关闭 1-开启
        /// </summary>
        public int IsExtraPrize { get; set; }

        /// <summary>
        /// 关联活动ID
        /// </summary>
        public Guid RelateActID { get; set; }

        /// <summary>
        /// 关联活动类型
        /// </summary>
        public int RelateActType { get; set; }

        /// <summary>
        ///关联活动主题
        /// </summary>
        public string RelateActTitle { get; set; }

        /// <summary>
        /// 免费次数
        /// </summary>
        public int FreeChance { get; set; }


    }
}