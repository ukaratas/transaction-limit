namespace bbt.enterprise_library.transaction_limit
{
    public class PathFormatException : BaseException
    {
        public PathFormatException()
        {
            statusInfo["message"] = "Invalid path format.";
            statusInfo["statusCode"] = "469";
        }
    }
}
