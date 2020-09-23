namespace bbt.enterprise_library.transaction_limit
{
    public class CurrencyConverterException : BaseException
    {
        public CurrencyConverterException()
        {
            statusInfo["message"] = "Exchange rates could not be obtained.";
            statusInfo["statusCode"] = "462";
        }
    }
}