namespace bbt.enterprise_library.transaction_limit
{
    public class MinimumMaximumException : BaseException
    {
        public MinimumMaximumException()
        {
            statusInfo["message"] = "Please check the minimum and maximum limit.";
            statusInfo["statusCode"] = "453";
        }
    }
}