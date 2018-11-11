using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketingAsync.Mongodb.Framework;
using MarketingAsync.Mtimes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MarketingAsync.Mongodb.Repository
{
    public class MtimeRepository : MongoRepository<Mtime>, IMtimeRepository
    {



        public IMongoCollection<BsonDocument> coll = Database.GetCollection<BsonDocument>("Analysis");

        public void WriteData(Analysis analysi)
        {
            BsonDocument bson = new BsonDocument();
            bson.Add("_id", analysi.Id);
            bson.Add("group", analysi.Group);
            bson.Add("parent", analysi.TodoParent);
            bson.Add("next", analysi.TodoChild);
            foreach (var index in analysi.KeyValuePairs.Value)
            {
                bson.Add(index.Key, index.Value);
            }
            coll.InsertOne(bson);


        }
    }
}
