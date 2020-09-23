namespace bbt.enterprise_library.transaction_limit
{
    public class PathNotDefinedException : BaseException
    {
        public PathNotDefinedException()
        {
            statusInfo["message"] = "Path is not defined.";
            statusInfo["statusCode"] = "460";
        }
    }
}
