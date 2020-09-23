namespace bbt.enterprise_library.transaction_limit
{
    public class NullPathException : BaseException
    {
        public NullPathException()
        {
            statusInfo["message"] = "Path can not be emtpy or null.";
            statusInfo["statusCode"] = "452";
        }
    }
}