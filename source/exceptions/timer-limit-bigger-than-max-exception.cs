namespace bbt.enterprise_library.transaction_limit
{
    public class TimerLimitBiggerThanMaxException : BaseException
    {
        public TimerLimitBiggerThanMaxException(LimitDefinition data)
        {
            statusInfo["message"] = data;
            statusInfo["statusCode"] = "470";
        }
    }
}