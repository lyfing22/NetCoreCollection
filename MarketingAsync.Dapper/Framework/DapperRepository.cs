using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Castle.Components.DictionaryAdapter;
using Dapper;

namespace MarketingAsync.Dapper.Framework
{

    public class DapperRepository<TEntity, T> : DapperRepositoriesBase<TEntity, T> where TEntity : Entity<T>
    {
        public string TableName { get; set; }

        public DapperRepository()
        {
            TableName = typeof(TEntity).Name;
        }


        public override List<TEntity> GetList(string where, object parament = null, string field = "*")
        {
            return ConnectionExcute(x => x.Query<TEntity>($"select {field} from [" + TableName + "] " + where, parament).ToList());
        }

        public override TEntity Get(T id)
        {
            return ConnectionExcute(x => x.Query<TEntity>("select top 1 * from [" + TableName + "] where id = '" + id + "'").FirstOrDefault());
        }

        public override TEntity Insert(TEntity entity)
        {
            List<string> list;
            var parament = GetParament(entity, out list);
            if (entity.Id != null)
            {
                parament.Add("@Id", entity.Id);
                list.Add("Id");
            }
            var sql = $"INSERT INTO [{TableName}] ({string.Join(",", list)}) VALUES (@{string.Join(",@", list)});SELECT @@IDENTITY";
            var id = ConnectionExcute(x => x.ExecuteScalar<T>(sql, parament));
            if (id != null)
            {
                entity.Id = id;
            }
            //trigger
            EventBus.Default.Trigger(new EntityCreatedEventData<TEntity>(entity));
            return entity;
        }


        public override TEntity Update(TEntity entity)
        {
            List<string> list;
            var parament = GetParament(entity, out list);
            var agg = list.Aggregate(new StringBuilder(), (x, y) =>
            {
                x.Append(y);
                x.Append("=@");
                x.Append(y);
                x.Append(",");
                return x;
            });
            var sql = $"UPDATE [{TableName}] SET {agg.ToString().TrimEnd(',')} WHERE ID = '{entity.Id}'";
            ConnectionExcute(x => x.Execute(sql, parament));
            //trigger
            EventBus.Default.Trigger(new EntityUpdatedEventData<TEntity>(entity));
            return entity;
        }

        public override void Delete(IEnumerable<TEntity> entities)
        {
            var sql = $"delete from [{ TableName }] where id in @Id";
            var list = entities.Select(x => x.Id);
            ConnectionExcute(x => x.Execute(sql, new { Id = list }));
        }

        public override void Delete(T id)
        {
            ConnectionExcute(x => x.Execute($"delete from [{TableName}] where id = '{id}'"));
            //根据Id删除

        }

        public override void Delete(string where, object parament = null)
        {
            var sql = $"delete from [{ TableName }] {where}";
            ConnectionExcute(x => x.Execute(sql, parament));
            //根据条件删除

        }

        public override int Count()
        {
            return ConnectionExcute(x => x.ExecuteScalar<int>("SELECT COUNT(1) FROM [" + TableName + "]"));
        }

        public override int Count(string where, object parament = null)
        {
            return ConnectionExcute(x => x.ExecuteScalar<int>("SELECT COUNT(1) FROM [" + TableName + "] " + where, parament));
        }


        public override TEntity FirstOrDefault(string where, object parament = null, string field = "*")
        {
            return ConnectionExcute(x => x.Query<TEntity>($" select top 1 {field} from [" + TableName + "] " + where, parament).FirstOrDefault());
        }

        public List<string> GetFieNameArray()
        {
            return (from node in typeof(T).GetProperties() where node.Name.ToLower() != "id" select node.Name).ToList();
        }

        //根据对象获取DynamicParament  
        public DynamicParameters GetParament(TEntity t, out List<string> list)
        {
            list = new List<string>();
            var parament = new DynamicParameters();
            foreach (var node in typeof(TEntity).GetProperties())
            {
                if (node.Name.ToLower() == "id" || node.GetValue(t) == null) continue;
                if (node.Name == "CreateTime")
                    node.SetValue(t, DateTime.Now);
                list.Add(node.Name);
                parament.Add("@" + node.Name, node.GetValue(t));
            }
            return parament;
        }
    }

    public class DapperRepository<TEntity> : DapperRepository<TEntity, int> where TEntity : Entity<int>
    {

    }

    public class DapperRepository : IDapperRepository
    {
        public virtual T ConnectionExcute<T>(Func<IDbConnection, T> func)
        {
            using (IDbConnection conn = new SqlConnection(PersistentConfigurage.MasterConnectionString))
            {
                return func(conn);
            }
        }


        public virtual T ConnectionExcute<T>(Func<IDbConnection, T> func, string conStr)
        {
            using (IDbConnection conn = new SqlConnection(conStr))
            {
                return func(conn);
            }
        }

        #region  

        public List<IEnumerable<object>> QueryMultiple(string sql, object p, params Type[] type)
        {
            List<IEnumerable<object>> list = new EditableList<IEnumerable<object>>();
            var data = ConnectionExcute(x =>
            {
                var procMultiple = x.QueryMultiple(sql, p);
                list.AddRange(type.Select(node => procMultiple.Read(node)));
                return list;
            });
            return list;
        }

        #endregion

        #region Sync

        public IEnumerable<TType> Query<TType>(string sql, object parament = null)
        {
            return ConnectionExcute(x => x.Query<TType>(sql, parament));
        }

        public int Excute(string sql, object parament = null)
        {
            return ConnectionExcute(x => x.Execute(sql, parament));
        }

        public void ExecProc(string procName, object parament = null, Action func = null)
        {
            ConnectionExcute(x => x.Execute(procName, parament, commandType: CommandType.StoredProcedure));
            func?.Invoke();
        }

        public IEnumerable<TModel> ExecProc<TModel>(string procName, object parament, Action func = null)
        {
            return ConnectionExcute(x => x.Query<TModel>(procName, parament, commandType: CommandType.StoredProcedure));
        }

        public TOutType ExecProc<TModel, TOutType>(string procName, object parament, Func<IEnumerable<TModel>, TOutType> func)
        {

            var data = ConnectionExcute(x => x.Query<TModel>(procName, parament, commandType: CommandType.StoredProcedure));
            return func(data);
        }

        public TOutType ExecProc<TModel, TOutType>(string procName, Func<IEnumerable<TModel>, TOutType> func)
        {
            return ExecProc(procName, null, func);
        }


        #endregion

        #region Async
        public Task<IEnumerable<TType>> QueryAsync<TType>(string sql, object parament = null)
        {
            return ConnectionExcute(x => x.QueryAsync<TType>(sql, parament));
        }

        public Task<int> ExcuteAsync(string sql, object parament = null)
        {
            return ConnectionExcute(x => x.ExecuteAsync(sql, parament));
        }

        public async Task ExecProcAsync(string procName, object parament = null, Action func = null)
        {
            await ConnectionExcute(x => x.ExecuteAsync(procName, parament, commandType: CommandType.StoredProcedure));
            func?.Invoke();
        }

        public Task<IEnumerable<TModel>> ExecProcAsync<TModel>(string procName, object parament, Action func = null)
        {
            return ConnectionExcute(x => x.QueryAsync<TModel>(procName, parament, commandType: CommandType.StoredProcedure));
        }

        public async Task<TOutType> ExecProcAsync<TModel, TOutType>(string procName, object parament, Func<IEnumerable<TModel>, TOutType> func)
        {
            var data = ConnectionExcute(x => x.QueryAsync<TModel>(procName, parament, commandType: CommandType.StoredProcedure));
            return func(await data);
        }

        public Task<TOutType> ExecProcAsync<TModel, TOutType>(string procName, Func<IEnumerable<TModel>, TOutType> func)
        {
            return ExecProcAsync(procName, null, func);
        }

        #endregion

        #region Paging

        public virtual List<TOutType> Pagination<TOutType>(
            string sql,
            int currentIndex,
            int pageSize,
            string translate = "*",
            string orderBy = "Id",
             object parament = null
            )
        {
            var excuteSql = GetPaginationSql(sql, currentIndex, pageSize, translate, orderBy);
            return ConnectionExcute(x => x.Query<TOutType>(excuteSql, parament).ToList());
        }

        public virtual PageEntity<TOutType> Pagination<TOutType>(string sql, int currentIndex, int pageSize, bool sumCount, string translate = "*", string orderBy = "Id", object parament = null)
        {
            var excuteSql = GetPaginationSql(sql, currentIndex, pageSize, translate, orderBy) + $";SELECT A.COUNT FROM ( SELECT COUNT(1) AS COUNT FROM {sql}) A";
            return ConnectionExcute(x =>
                {
                    var read = x.QueryMultiple(excuteSql, parament);
                    var page = new PageEntity<TOutType>
                    {
                        EntityList = read.Read<TOutType>().ToList(),
                        Count = read.Read<int>().Single()
                    };
                    return page;
                });
        }

        public static string GetPaginationSql(
            string sql,
            int currentIndex,
            int pageSize,
            string translate = "*",
            string orderBy = "Id"
           )
        {
            var start = currentIndex == 0 || currentIndex == 1 ? 1 : ((currentIndex - 1) * pageSize) + 1;
            return $"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {orderBy})NewRow ,{translate} FROM {sql} ) AUS WHERE NewRow BETWEEN {start} AND {start + pageSize - 1}";
        }

        #endregion

    }

}