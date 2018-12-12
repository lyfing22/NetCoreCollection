using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;

namespace MarketingAsync.MySql
{
    public class PublisherEntity : Entity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<BookEntity> Books { get; set; }
    }
}
