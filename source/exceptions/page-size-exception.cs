namespace bbt.enterprise_library.transaction_limit
{
    public class PageSizeException : BaseException
    {
        public PageSizeException()
        {
            statusInfo["statusCode"] = "453";
        }
    }
}