using System.Collections.Generic;
using EventAwardRecordsEntity = MarketingAsync.Act.User.EventAwardRecordsEntity;

namespace MarketingAsync.Act.User
{
    /// <summary>
    /// 用户签到信息
    /// </summary>
    public interface IUserSignActivityRepository : IMongoRepository<UserSignActivity>
    {
        
    }
}