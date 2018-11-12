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

        public void updateData(Analysis analysi)
        {
            //var bson = new BsonDocument
            //{
            //    {"_id", analysi.Id},
            //    {"group", analysi.Group},
            //    {"parent", analysi.TodoParent},
            //    {"now",analysi.KeyValuePairs.Key } ,
            //    {"next", analysi.TodoChild},
            //    {"total", analysi.Total}
            //};
            //foreach (var index in analysi.KeyValuePairs.Value)
            //{
            //    bson.Add(index.Key, index.Value);
            //}

            var update = Builders<BsonDocument>.Update.Set("now", analysi.KeyValuePairs.Key).Set("total", analysi.Total);

            coll.UpdateOne($"{{'_id':'{analysi.Id}'}}", update);
        }


    }

    /*
    public void updateData(Analysis analysi)
    {
        var bson = new BsonDocument
        {
            {"_id", analysi.Id},
            {"group", analysi.Group},
            {"parent", analysi.TodoParent},
            {"now", analysi.KeyValuePairs.Key} ,
            {"next", analysi.TodoChild},
            {"total", analysi.Total}
        };
        foreach (var index in analysi.KeyValuePairs.Value)
        {
            bson.Add(index.Key, index.Value);
        }
        coll.InsertOne(bson);


    }*/

}
