using System;
using System.Collections.Generic;
using System.Text;
using MarketingAsync.Dapper;


namespace MarketingAsync.MySql
{
    public interface IBookRepository : IDapperRepository<BookEntity>
    {
    }
}
