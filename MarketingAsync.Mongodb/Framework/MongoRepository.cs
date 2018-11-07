using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MarketingAsync.Mongodb.Framework
{
    public class MongoRepository<TEntity, TPrimaryKey> : MongodbBase<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {

        public MongoRepository()
        {
        }

        /// <summary>
        /// 数据库
        /// </summary>
        protected static IMongoDatabase Database => new MongoClient(DocumentConfigurage.ConnectionString).GetDatabase(DocumentConfigurage.DatabaseMongoDB);

        /// <summary>
        /// 数据连接池
        /// </summary>
        protected static IMongoCollection<TEntity> Collection
        {
            get
            {
                var entity = typeof(TEntity).Name;
                return Database.GetCollection<TEntity>(entity);
            }
        }

        public override IQueryable<TEntity> Queryable()
        {
            return Collection.AsQueryable();
        }


        public override TEntity FirstOrDefault(TPrimaryKey id)
        {
            return Queryable().FirstOrDefault(x => Equals(x.Id, id));
        }

        public override TEntity Insert(TEntity entity)
        {
            Collection.InsertOne(entity);
            // EventBus.Default.Trigger(new EntityCreatedEventData<TEntity>(entity));
            return entity;
        }

        public override async Task<TEntity> InsertAsync(TEntity entity)
        {
            await Collection.InsertOneAsync(entity);
            // await EventBus.Default.TriggerAsync(new EntityCreatedEventData<TEntity>(entity));
            return entity;
        }

        public override IEnumerable<TEntity> InsertList(IEnumerable<TEntity> list)
        {
            Collection.InsertMany(list);
            return list ?? new List<TEntity>();
        }



        public override void Delete(TPrimaryKey id)
        {
            var dictionary = new Dictionary<string, TPrimaryKey> { { "_id", id } };
            var query = new BsonDocument(dictionary);
            Collection.DeleteOneAsync(query);
        }


        public override long Update(TPrimaryKey id, object fields)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);
            var updateDefinitionList = new List<UpdateDefinition<TEntity>>();
            var propertieArr = fields.GetType().GetProperties();
            foreach (var proper in propertieArr)
            {
                object properValue = proper.GetValue(fields);
                updateDefinitionList.Add(Builders<TEntity>.Update.Set(proper.Name, properValue));
            }
            var updateDefinition = new UpdateDefinitionBuilder<TEntity>().Combine(updateDefinitionList);
            var result = Collection.UpdateOne(filter, updateDefinition);
            return result.MatchedCount;

        }

    }

}
