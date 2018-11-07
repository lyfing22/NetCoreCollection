using MarketingAsync.Act.User;
using MarketingAsync.Mongodb.Framework;

namespace MarketingAsync.Mongodb.Repository
{
    /// <summary>
    /// 用户签到信息
    /// </summary>
    public class UserSignActivityRepository : MongoRepository<UserSignActivity, string>, IUserSignActivityRepository
    {

    }
}