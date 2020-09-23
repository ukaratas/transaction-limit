namespace bbt.enterprise_library.transaction_limit
{
    public class TimerRemainingLimitException : BaseException
    {
        public TimerRemainingLimitException(LimitDefinition data)
        {
            statusInfo["statusCode"] = "454";
            statusInfo["message"] = data;
        }
    }
}
