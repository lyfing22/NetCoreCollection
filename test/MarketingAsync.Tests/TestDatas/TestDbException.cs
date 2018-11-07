using System;

namespace MarketingAsync.Tests.TestDatas
{
    public class TestDbException : Exception
    {
        public override string Message { get; }

        public TestDbException(string dbType)
        {
            Message = dbType + "连接有问题";

        }
    }
}