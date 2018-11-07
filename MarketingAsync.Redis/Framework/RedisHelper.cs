using System;
using System.Collections.Generic;
using System.Linq;
using MarketingAsync.Redis;
using MarketingAsync.Redis.Cache;
using StackExchange.Redis;

namespace Cloud.Redis.Framework
{
    public class RedisHelper : IRedisHelper, IKeyAppointment
    {
        private static ConnectionMultiplexer _redis;

        private static readonly object Locker = new object();

        protected static ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (Locker)
                    {
                        if (_redis != null) return _redis;
                        _redis = ConnectionMultiplexer.Connect(CacheConfigurage.ConnectionString);
                        return _redis;
                    }
                }
                return _redis;
            }
        }

        #region

        public string HashGet(string key, string hashField, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.HashGet(key, hashField);
        }

        public Dictionary<string, string> HashGetAll(string key, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            var date = redis.HashGetAll(key);
            return date.ToDictionary<HashEntry, string, string>(hashEntry => hashEntry.Name, hashEntry => hashEntry.Value);
        }

        public string[] HashGet(string key, IEnumerable<string> hashFields, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            var date = redis.HashGet(key, hashFields.Select(hashField => (RedisValue)hashField).ToArray());
            return date.Select(redisValue => (string)redisValue).ToArray();
        }

        //待测试
        public void HashSet(string key, IEnumerable<KeyValueStruct> entry, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            var redisEntity = entry.Select(node => new HashEntry(node.Name, node.Value ?? "")).ToArray();
            redis.HashSet(key, redisEntity);
        }

        public void HashSet<TType>(string key, TType type, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            var propertyLst = typeof(TType).GetProperties();
            var redisEntity = propertyLst.Select(property => new HashEntry(property.Name, property.GetValue(type).ToString())).ToArray();
            redis.HashSet(key, redisEntity);
        }

        public void HashSet(string key, string hashField, string value, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            redis.HashSet(key, hashField, value);
        }


        public long HashIncrement(string key, string hashField, long value = 1, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.HashIncrement(key, value, value);
        }
        public long HashDecrement(string key, string hashField, long value = 1, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.HashDecrement(key, value, value);

        }
        public bool HashDelete(string key, string value, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.HashDelete(key, value);
        }
        public bool HashDelete(string key, IEnumerable<string> value, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.HashDelete(key, value.Select(x => (RedisValue)x).ToArray()) > 0;
        }

        #endregion

        #region All

        public void KeyDelete(string key, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            redis.KeyDelete(key);
        }

        public void KeyDelete(string[] key, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            var rediskey = key.Select(s => (RedisKey)s).ToArray();
            redis.KeyDelete(rediskey);
        }

        public bool KeyExists(string key, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.KeyExists(key);
        }

        public void FlushDb(int database = 0)
        {
            var keys = Keys();
            var redis = Manager.GetDatabase(database);
            foreach (var key in keys)
            {
                redis.KeyDelete(key);
            }
        }

        public void FlushAll()
        {
            var server = Manager.GetServer(CacheConfigurage.ConnectionString);
            server.FlushAllDatabases();
        }

        public List<string> Keys(int database = 0)
        {
            var server = Manager.GetServer(CacheConfigurage.ConnectionString);
            return server.Keys(database).Select(key => (string)key).ToList();
        }

        public List<string> Keys(string searchKey, int database = 0)
        {
            var server = Manager.GetServer(CacheConfigurage.ConnectionString);
            return server.Keys(database, searchKey).Select(key => (string)key).ToList();
        }

        #endregion 

        #region String

        public string StringGet(string key, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.StringGet(key);
        }

        public bool StringSet(string key, string value, int expireSecond = 60, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.StringSet(key, value, TimeSpan.FromSeconds(expireSecond));
        }

        public long StringIncrement(string key, long value, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.StringIncrement(key, value);
        }

        #endregion

        #region Set


        public bool SetAdd(string key, string value, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.SetAdd(key, value);
        }

        public bool SetRemove(string key, string value, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.SetRemove(key, value);
        }
        public long SetLength(string key, string value, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.SetLength(key);
        }

        public List<string> SetMembers(string key, string value, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.SetMembers(key).Select(x => x.ToString()).ToList();
        }
        public string SetPop(string key, string value, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.SetPop(key);
        }
        #endregion

        #region List

        public long ListRightPush(string key, string value, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.ListRightPush(key, value);
        }

        public string[] ListRange(string key, long start, long end, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.ListRange(key, start, end).Select(x => x.ToString()).ToArray();
        }

        public long ListLength(string key, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.ListLength(key);
        }

        public string ListLeftPop(string key, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.ListLeftPop(key);
        }

        public string ListRightPop(string key, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            return redis.ListRightPop(key);
        }
        #endregion

        #region Other

        public void KeyExpire(string key, long time, int database = 0)
        {
            var redis = Manager.GetDatabase(database);
            redis.KeyExpire(key, DateTime.Now.AddSeconds(time));
        }

        #endregion

        public virtual string Merge(string key)
        {
            return CurrentKey + ":" + key;
        }

        public virtual string CurrentKey { get; }
    }
}