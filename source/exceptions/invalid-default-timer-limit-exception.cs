namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidDefaultTimerLimitException : BaseException
    {
        public InvalidDefaultTimerLimitException()
        {
            statusInfo["statusCode"] = "473";
        }
    }
}