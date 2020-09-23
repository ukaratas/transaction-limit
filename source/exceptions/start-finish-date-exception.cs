namespace bbt.enterprise_library.transaction_limit
{
    public class StartFinishDateException : BaseException
    {
        public StartFinishDateException()
        {
            statusInfo["message"] = "Please check the exception dates.";
            statusInfo["statusCode"] = "463";
        }
    }
}