using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Dependency;
using MarketingAsync.Act;

namespace MarketingAsync.Mongodb
{
    /// <summary>
    /// 审计日志
    /// </summary>
    public class AuditingStore : IAuditingStore, ISingletonDependency
    {


        public Task SaveAsync(AuditInfo auditInfo)
        {
            return Task.FromResult(1);
        }
    }
}