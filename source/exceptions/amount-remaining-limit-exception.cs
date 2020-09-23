namespace bbt.enterprise_library.transaction_limit
{
    public class AmountRemainingLimitException : BaseException
    {
        public AmountRemainingLimitException(LimitDefinition data)
        {
            statusInfo["statusCode"] = "457";
            statusInfo["message"] = data;
        }
    }
}