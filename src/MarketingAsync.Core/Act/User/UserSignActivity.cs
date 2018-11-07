using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using MarketingAsync.Act.SigException;
using MongoDB.Bson.Serialization.Attributes;

namespace MarketingAsync.Act.User
{
    /// <summary>
    /// 
    /// </summary>
    public class UserSignActivity : Entity<string>
    {
        /// <inheritdoc />
        /// <summary>
        /// 创建一个新的用户签到记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="actId"></param>
        public UserSignActivity(string userId, string actId) : this()
        {
            this.UserId = userId;
            this.ActivityId = actId;
            this.Id = GetId(userId, actId);

        }
        /// <summary>
        /// 活动用户签到表
        /// </summary>
        public UserSignActivity()
        {
            MemberSignatureCard = new List<MemberSignatureCardEntity>();
            SignRecords = new List<SignRecordsEntity>();
            SignSpecialRecords = new List<SignSpecialRecordsEntity>();
            EventAwardRecords = new List<EventAwardRecordsEntity>();
        }

        /// <summary>
        /// 获取用户在这活动下的签到记录ID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="actId"></param>
        /// <returns></returns>
        /// <exception cref="ParameterNonstandard"></exception>
        public static string GetId(string userId, string actId)
        {
            if (userId == null || actId == null)
            {
                throw new ParameterNonstandard("userId" + userId, "actId:" + actId);
            }
            return userId + actId;
        }


        /// <summary>
        /// 用户在此次活动中的唯一标识(用户id+活动id)
        /// </summary>
        public sealed override string Id { get; set; }

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 活动唯一标识
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// 历史最大连续签到数
        /// </summary>
        public int MaxTotalNumber { get; set; }

        /// <summary>
        /// 最后签到时间
        /// </summary> 
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastSignTime { get; set; }


        /// <summary>
        /// 会员补签卡表
        /// </summary>
        public IEnumerable<MemberSignatureCardEntity> MemberSignatureCard { get; set; }

        /// <summary>
        /// 签到记录表
        /// </summary>
        public IEnumerable<SignRecordsEntity> SignRecords { get; set; }

        /// <summary>
        /// 特殊规则记录表
        /// </summary>
        public IEnumerable<SignSpecialRecordsEntity> SignSpecialRecords { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<EventAwardRecordsEntity> EventAwardRecords { get; set; }

    }
}