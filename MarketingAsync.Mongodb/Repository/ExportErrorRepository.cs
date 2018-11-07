using System;
using System.Collections.Generic;
using System.Text;
using MarketingAsync.Act.Act;
using MarketingAsync.Mongodb.Framework;

namespace MarketingAsync.Mongodb.Repository
{
    public class ExportErrorRepository: MongoRepository<ExportError, string>, IExportErrorRepository
    {
    }
}
