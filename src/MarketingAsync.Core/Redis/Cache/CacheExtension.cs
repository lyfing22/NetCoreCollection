using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Entities;
using MarketingAsync.Dapper;
using Newtonsoft.Json;

namespace MarketingAsync.Redis.Cache
{
    public static class CacheExtension
    {


        private static readonly object RedisLocker = new object();
        private static readonly object DapperLocker = new object();
        private static IRedisModelStrategy _redisModelStrategy;
        private static IDapperRepository _dapperRepositorie;

        private static IRedisModelStrategy Manager()
        {
            if (_redisModelStrategy == null)
            {
                lock (RedisLocker)
                {
                    if (_redisModelStrategy != null)
                        return _redisModelStrategy;
                    return _redisModelStrategy = IocManager.Instance.Resolve<IRedisModelStrategy>();
                }
            }
            return _redisModelStrategy;
        }

        private static IDapperRepository ManagerRepositorie()
        {
            if (_dapperRepositorie == null)
            {
                lock (DapperLocker)
                {
                    if (_dapperRepositorie != null)
                        return _dapperRepositorie;
                    return _dapperRepositorie = IocManager.Instance.Resolve<IDapperRepository>();

                }
            }
            return _dapperRepositorie;
        }


        public static void Replace(IRedisModelStrategy redis, IDapperRepository dapperRepositories)
        {
            _redisModelStrategy = redis;
            _dapperRepositorie = dapperRepositories;
        }

        /// <summary>
        /// 批量获取对象信息 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static List<TModel> GetModelList<TModel, TId>(this List<TId> idList) where TModel : IEntity<TId>
        {
            var key = typeof(TModel).Name;
            if (idList == null || idList.Count == 0)
            {
                return new List<TModel>();
            }
            var list = Manager().GetModelAll(key, idList.Select(x => x.ToString()).ToArray()).ToList();
            if (list.Count == 0 || list.Count != idList.Count)
            {
                var getValue = ManagerRepositorie().Query<TModel>($"select * from [{key}] where id in ({string.Join(",", idList)})").ToList();
                Manager().SetModelAll<TModel, TId>(key, getValue);
                return getValue;
            }
            var newValue = list.Select(JsonConvert.DeserializeObject<TModel>);
            return newValue.ToList();

        }

        public static TModel GetModel<TModel, TId>(this TModel entity) where TModel : IEntity<TId>
        {
            return GetModel<TModel, TId>(entity.Id);
        }

        public static TModel GetModel<TModel, TId>(TId id) where TModel : IEntity<TId>
        {
            var key = typeof(TModel).Name;
            var model = Manager().GetModel<TModel>(key, id.ToString());
            if (model == null)
            {
                model = ManagerRepositorie().Query<TModel>($"select * from [{key}] where id = @id", new { id }).FirstOrDefault();
                if (model != null)
                {
                    Manager().SetModel(key, id.ToString(), model);
                }
            }
            return model;
        }


        public static void UpdateModel<TModel>(this TModel model) where TModel : IEntity
        {
            CheckModel(model);
            Manager().SetModel(typeof(TModel).Name, model.Id.ToString(), model);
        }

        public static void UpdateListModel<TModel, TId>(this IEnumerable<TModel> models) where TModel : IEntity<TId>
        {
            Manager().SetModelAll<TModel, TId>(typeof(TModel).Name, models);
        }

        public static void SetModel<TModel>(this TModel model) where TModel : IEntity
        {
            CheckModel(model);
            Manager().SetModel(typeof(TModel).Name, model.Id.ToString(), model);
        }


        private static void CheckModel<TModel>(this TModel model) where TModel : IEntity
        {
            if (model == null || model.Id == 0)
                throw new Exception("实体不能为空，或者实体Id不能为空！");
        }

        ///// <summary>
        ///// 清除整个Key,已过期
        ///// </summary>
        ///// <typeparam name="TModel"></typeparam>
        ///// <param name="model"></param> 
        //public static void RemoveCache<TModel>(this TModel model) where TModel : IEntity
        //{
        //    Manager().Clear(typeof(TModel).Name);
        //}

        public static void RemoveCache<TModel, TId>(TId id) where TModel : IEntity<TId>
        {
            Manager().HashClear(typeof(TModel).Name, id.ToString());
        }

        /// <summary>
        /// 移除实体列表
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        public static void RemoveCache<TModel>(this IEnumerable<TModel> model) where TModel : IEntity
        {
            Manager().HashClear(typeof(TModel).Name, model.Select(x => x.Id.ToString()).ToList());
        }

        /// <summary>
        /// 根据Keyvalue移除实体
        /// </summary>
        /// <param name="key"></param>
        /// <param name="id"></param>
        public static void RemoveCache(string key, string id)
        {
            Manager().HashClear(key, id);
        }

    }
}
