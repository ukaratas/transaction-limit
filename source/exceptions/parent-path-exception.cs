namespace bbt.enterprise_library.transaction_limit
{
    public class ParentPathException : BaseException
    {
        public ParentPathException()
        {
            statusInfo["message"] = "This path is parent.";
            statusInfo["statusCode"] = "461";
        }
    }
}