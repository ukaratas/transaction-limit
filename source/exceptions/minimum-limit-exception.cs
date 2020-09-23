namespace bbt.enterprise_library.transaction_limit
{
    public class MinimumLimitException : BaseException
    {
        public MinimumLimitException(LimitDefinition data)
        {
            statusInfo["statusCode"] = "455";
            statusInfo["message"] = data;
        }
    }
}
