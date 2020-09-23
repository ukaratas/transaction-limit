namespace bbt.enterprise_library.transaction_limit
{
    public class AvailabilityExceptionException : BaseException
    {
        public AvailabilityExceptionException()
        {
            statusInfo["message"] = "No transaction on holidays.";
            statusInfo["statusCode"] = "463";
        }
    }
}

