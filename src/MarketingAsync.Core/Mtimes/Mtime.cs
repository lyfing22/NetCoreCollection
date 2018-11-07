using System;

namespace MarketingAsync.Mtimes
{
    public class Mtime
    {

        /// <summary>
        /// 计时器组名
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 关联的数据id
        /// </summary>
        public string DataId { get; set; }

        /// <summary>
        /// 消息情况
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 计时时间
        /// </summary>
        public DateTime Timespan { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public int Order { get; set; }

        public Mtime(string group, string actid, string message = "")
        {
            this.Group = group;
            this.DataId = actid;
            this.Message = message;
            this.Timespan = DateTime.Now;
            this.Order += 1;
        }


    }
}