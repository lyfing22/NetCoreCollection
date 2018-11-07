using System.Collections.Generic;
using Abp.Domain.Entities;

namespace MarketingAsync.Redis
{
    public interface IRedisModelStrategy  
    {
        string MergeKey(string key);

        #region Get

        T GetModel<T>(string key, string id);

        string GetModel(string key, string id);

        #endregion

        #region Set

        void SetModel(string key, string id, string value);

        void SetModel<T>(string key, string id, T value);
        /// <summary>
        /// 自指向设置模型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        void SetModel(string key, string id, object value);

        #endregion

        #region GetAll

        List<T> GetModelAll<T>(string key, IEnumerable<string> id);

        IEnumerable<string> GetModelAll(string key, IEnumerable<string> id);


        #endregion

        #region SetAll

        void SetModelAll(string key, IEnumerable<KeyValueStruct> value);

        void SetModelAll(string key, Dictionary<string, string> value);

        void SetModelAll<T, TId>(string key, IEnumerable<T> list) where T : IEntity<TId>;

        #endregion

        #region Clear

        void Clear(IEnumerable<string> keys);

        void Clear(string key);

        void HashClear(string key, string id);

        void HashClear(string key, List<string> id);

        #endregion
    }
}