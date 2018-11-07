using System;
using System.Collections.Generic;
using System.Text;
using MarketingAsync.Dapper;

namespace MarketingAsync.Act.Sqlserver
{
    public interface ISignCardStaticRepository : IDapperRepository<fact_Statistic_SignatureCard>
    {
    }
}
