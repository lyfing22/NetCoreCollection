using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketingAsync.Mongodb.Framework;
using MarketingAsync.Mtimes;
using MongoDB.Bson;

namespace MarketingAsync.Mongodb.Repository
{
    public class MtimeRepository : MongoRepository<Mtime>, IMtimeRepository
    {
        public void InsertList(Mtime[] str)
        {
            Collection.InsertMany(str);

        }
    }
}
