using System.Collections.Generic;

namespace MarketingAsync.Dapper
{
    public class PageEntity<T>
    {
        public List<T> EntityList { get; set; }

        public int Count { get; set; }

    }
}