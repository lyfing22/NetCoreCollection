using System;

namespace MarketingAsync.Act.SigException
{
    public class MarketingException : Exception
    {
        public MarketingException() { }
        public MarketingException(string ms) : base(ms)
        { }
    }
}