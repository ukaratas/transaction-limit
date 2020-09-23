namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidDefaultAmountLimitException : BaseException
    {
        public InvalidDefaultAmountLimitException()
        {
            statusInfo["statusCode"] = "472";
        }
    }
}