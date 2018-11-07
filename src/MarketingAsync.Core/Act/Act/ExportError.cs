using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MarketingAsync.Act.Act
{
    public class ExportError : Entity<string>
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public string RowId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateTime { get; set; }=DateTime.Now;

        public string ErrorMsg { get; set; }

        public string OperateException { get; set; }
    }
}
