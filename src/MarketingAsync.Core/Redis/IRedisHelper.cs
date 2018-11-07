using System.Collections.Generic;
using Abp.Dependency;

namespace MarketingAsync.Redis
{
    public interface IRedisHelper : ISingletonDependency
    {
        #region

        string HashGet(string key, string hashField, int database = 0);

        Dictionary<string, string> HashGetAll(string key, int database = 0);

        string[] HashGet(string key, IEnumerable<string> hashFields, int database = 0);

        void HashSet(string key, IEnumerable<KeyValueStruct> entry, int database = 0);

        void HashSet<TType>(string key, TType type, int database = 0);

        void HashSet(string key, string hashField, string value, int database = 0);


        long HashIncrement(string key, string hashField, long value = 1, int database = 0);

        long HashDecrement(string key, string hashField, long value = 1, int database = 0);

        bool HashDelete(string key, string value, int database = 0);

        bool HashDelete(string key, IEnumerable<string> value, int database = 0);

        #endregion

        #region All

        void KeyDelete(string key, int database = 0);

        void KeyDelete(string[] key, int database = 0);

        bool KeyExists(string key, int database = 0);

        void FlushDb(int database = 0);

        void FlushAll();

        List<string> Keys(int database = 0);

        List<string> Keys(string searchKey, int database = 0);


        #endregion

        #region String

        string StringGet(string key, int database = 0);

        bool StringSet(string key, string value, int expireSecond,int database = 0);
        /// <summary>
        /// 递增递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="database"></param>
        /// <returns></returns>

        long StringIncrement(string key, long value, int database = 0);

        #endregion

        #region Set


        bool SetAdd(string key, string value, int database = 0);

        bool SetRemove(string key, string value, int database = 0);

        long SetLength(string key, string value, int database = 0);

        List<string> SetMembers(string key, string value, int database = 0);

        string SetPop(string key, string value, int database = 0);

        #endregion

        #region List

        long ListRightPush(string key, string value, int database = 0);

        string[] ListRange(string key, long start, long end, int database = 0);

        long ListLength(string key, int database = 0);

        string ListLeftPop(string key, int database = 0);

        string ListRightPop(string key, int database = 0);


        #endregion

        #region Other
        /// <summary>
        /// 指定过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time"></param>
        /// <param name="database"></param>
        void KeyExpire(string key, long time, int database = 0);

        #endregion

    }
}