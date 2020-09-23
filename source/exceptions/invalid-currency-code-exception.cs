namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidCurrencyCodeException : BaseException
    {
        public InvalidCurrencyCodeException()
        {
            statusInfo["message"] = "Currency code is invalid.";
            statusInfo["statusCode"] = "458";
        }
    }
}