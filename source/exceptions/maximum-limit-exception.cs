namespace bbt.enterprise_library.transaction_limit
{
    public class MaximumLimitException : BaseException
    {
        public MaximumLimitException(LimitDefinition data)
        {
            statusInfo["statusCode"] = "456";
            statusInfo["message"] = data;
        }
    }
}
