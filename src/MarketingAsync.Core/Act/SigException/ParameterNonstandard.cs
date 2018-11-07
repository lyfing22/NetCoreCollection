namespace MarketingAsync.Act.SigException
{
    /// <summary>
    /// 参数不正确
    /// </summary>
    public class ParameterNonstandard : MarketingException
    {

        public override string Message { get; }

        public ParameterNonstandard(params object[] ps)
        {
            var message = string.Join(",", ps);
            Message = message;
        }

    }
}