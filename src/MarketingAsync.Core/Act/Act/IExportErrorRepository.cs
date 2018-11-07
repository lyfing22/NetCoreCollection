using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;

namespace MarketingAsync.Act.Act
{
  public   interface IExportErrorRepository : IMongoRepository<ExportError>, ISingletonDependency
    {
    }
}
