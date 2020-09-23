namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidMaxAmountLimitException : BaseException
    {
        public InvalidMaxAmountLimitException()
        {
            statusInfo["message"] = "Invalid maximum amount limit.";
            statusInfo["statusCode"] = "465";
        }
    }
}