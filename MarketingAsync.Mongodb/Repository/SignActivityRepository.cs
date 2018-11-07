using MarketingAsync.Act.Act;
using MarketingAsync.Mongodb.Framework;

namespace MarketingAsync.Mongodb.Repository
{
    /// <summary>
    /// 活动仓储
    /// </summary>
    public class SignActivityRepository : MongoRepository<SignActivity, string>, ISignActivityRepository
    {

    }
}