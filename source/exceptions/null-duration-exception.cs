namespace bbt.enterprise_library.transaction_limit
{
    public class NullDurationException : BaseException
    {
        public NullDurationException()
        {
            statusInfo["message"] = "Duration can not be emtpy or null.";
            statusInfo["statusCode"] = "455";
        }
    }
}