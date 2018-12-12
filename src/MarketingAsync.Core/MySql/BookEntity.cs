using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;

namespace MarketingAsync.MySql
{
    public class BookEntity: Entity
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Language { get; set; }
        public int Pages { get; set; }
        public virtual PublisherEntity Publisher { get; set; }
    }
}
