using System;

namespace MarketingAsync.Mtimes
{
    public class TimeAnchorsException : Exception
    {

        public new string Message { get; set; }
        public TimeAnchorsException(string mess)
        {
            this.Message = mess;
        }
    }
}