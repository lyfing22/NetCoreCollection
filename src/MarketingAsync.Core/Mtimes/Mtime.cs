using System;
using Abp.Domain.Entities;

namespace MarketingAsync.Mtimes
{
    public class Mtime : Entity<string>
    {

        public override string Id { get; set; }

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

        public int TotalTime { get; set; }

        public int CurrentTime { get; set; }


        /// <summary>
        /// 顺序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActId { get; set; }


        private static DateTime? TotalNow = null;

        private static DateTime? currentNow = null;

        private static bool isResetCurrentNow { set; get; } = true;

        public static void ResetCurrentNow(bool reset = true)
        {
            isResetCurrentNow = true;
        }

        private int GetTotalMillisecond()
        {
            if (TotalNow == null)
            {
                TotalNow = DateTime.Now;
            }
            return Convert.ToInt32((DateTime.Now - TotalNow.Value).TotalMilliseconds);
        }

        private int GetCurrentMillisecond()
        {

            if (isResetCurrentNow)
            {
                currentNow = DateTime.Now;
                isResetCurrentNow = false;
            }

            return Convert.ToInt32((DateTime.Now - currentNow.Value).TotalMilliseconds);
        }


        public Mtime(int order, string group, string actid, string message = "")
        {
            this.Id = Guid.NewGuid().ToString().Replace("-", "");
            this.Group = group;
            this.DataId = actid;
            this.Message = message;
            this.TotalTime = GetTotalMillisecond();
            this.CurrentTime = GetCurrentMillisecond();
            this.Order = order;
        }


    }
}