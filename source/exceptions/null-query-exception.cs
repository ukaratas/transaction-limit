namespace bbt.enterprise_library.transaction_limit
{
    public class NullQueryException : BaseException
    {
        public NullQueryException()
        {
            statusInfo["statusCode"] = "452";
        }
    }
}