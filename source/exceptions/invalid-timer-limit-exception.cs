namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidTimerLimitException : BaseException
    {
        public InvalidTimerLimitException()
        {
            statusInfo["message"] = "Invalid timer limit. Cannot be less than 0 '-1' is an exception.";
            statusInfo["statusCode"] = "454";
        }
    }
}
