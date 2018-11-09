using System;
using MarketingAsync.Act.Act;
using MarketingAsync.Mongodb.Framework;
using MongoDB.Bson;

namespace MarketingAsync.Mongodb.Repository
{
    /// <summary>
    /// 活动仓储
    /// </summary>
    public class SignActivityRepository : MongoRepository<SignActivity>, ISignActivityRepository
    {
        public Tuple<long, long> ClearData(bool error)
        {
            var sd = Collection.DeleteMany("{}");
            var coll = Database.GetCollection<BsonDocument>("UserSignActivity");
            var ud = coll.DeleteMany("{}");
            return new Tuple<long, long>(sd.DeletedCount, ud.DeletedCount);

        }
    }
}