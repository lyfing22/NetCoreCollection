using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;

namespace MarketingAsync.Act.Sqlserver
{
    public class fact_Statistic_SignatureCard: Entity
    {
        /// <summary>
        /// 活动id,主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public string RQ { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public string Memberlogin { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public int ActivityID { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public string Openid { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public int UseNum { get; set; }



        /// <summary>
        ///  
        /// </summary>
        public int IsDelete { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public int InvitationNum { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public string ActID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerNickName { get; set; }
        public string CustomerHeadPath { get; set; }
    }
}
