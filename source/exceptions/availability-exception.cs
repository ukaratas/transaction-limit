namespace bbt.enterprise_library.transaction_limit
{
    public class AvailabilityException : BaseException
    {
        public AvailabilityException(AvailabilityRejectDefinition data)
        {
            statusInfo["statusCode"] = "459";
            statusInfo["message"] = data;
        }
    }
}

