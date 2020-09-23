namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidAmountLimitException : BaseException
    {
        public InvalidAmountLimitException()
        {
            statusInfo["message"] = "Invalid amount limit.";
            statusInfo["statusCode"] = "468";
        }
    }
}