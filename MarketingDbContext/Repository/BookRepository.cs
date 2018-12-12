using System;
using System.Collections.Generic;
using System.Text;
using MarketingAsync.MySql;

namespace MarketingAsync.EFCore.Repository
{
    public class BookRepository:MysqlRepositoryBase<BookEntity,int>
    {
    }
}
