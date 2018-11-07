namespace MarketingAsync.Act.SigException
{
    /// <summary>
    /// 表达式错误
    /// </summary>
    public class MongodbExpressionException : SignException
    {
        public override string Message { get; }


        public MongodbExpressionException(string message)
        {
            this.Message = message;
        }

    }
}