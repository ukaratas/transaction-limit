namespace bbt.enterprise_library.transaction_limit
{
    public class PageIndexException : BaseException
    {
        public PageIndexException()
        {
            statusInfo["statusCode"] = "454";
        }
    }
}