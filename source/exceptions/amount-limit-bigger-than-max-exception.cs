namespace bbt.enterprise_library.transaction_limit
{
    public class AmountLimitBiggerThanMaxException : BaseException
    {
        public AmountLimitBiggerThanMaxException(LimitDefinition data)
        {
            statusInfo["statusCode"] = "466";
            statusInfo["message"] = data;
        }
    }
}