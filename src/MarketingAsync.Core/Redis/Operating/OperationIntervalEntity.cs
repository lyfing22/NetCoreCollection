using Abp.Domain.Entities;

namespace MarketingAsync.Redis.Operating
{
    /// <summary>
    /// 运行区间实体
    /// </summary>
    public class OperationIntervalEntity : Entity<string>
    {
        //用户Id
        public string UserId { get; set; }
        //并行增量(每秒/次)
        public long Count { get; set; }

    }
}
