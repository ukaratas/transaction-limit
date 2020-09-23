namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidMinimumLimitException : BaseException
    {
        public InvalidMinimumLimitException()
        {
            statusInfo["message"] = "Please check the minimum limit.";
            statusInfo["statusCode"] = "456";
        }
    }
}