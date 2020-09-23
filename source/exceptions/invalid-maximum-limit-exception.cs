namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidMaximumLimitException : BaseException
    {
        public InvalidMaximumLimitException()
        {
            statusInfo["message"] = "Please check the maximum limit.";
            statusInfo["statusCode"] = "455";
        }
    }
}