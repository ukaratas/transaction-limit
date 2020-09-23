namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidMaxTimerLimitException : BaseException
    {
        public InvalidMaxTimerLimitException()
        {
            statusInfo["message"] = "Invalid maximum timer limit.";
            statusInfo["statusCode"] = "471";
        }
    }
}