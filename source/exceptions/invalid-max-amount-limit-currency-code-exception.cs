namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidMaxAmountLimitCurrencyCodeException : BaseException
    {
        public InvalidMaxAmountLimitCurrencyCodeException()
        {
            statusInfo["message"] = "Max amount limit currency code is invalid.";
            statusInfo["statusCode"] = "467";
        }
    }
}