namespace bbt.enterprise_library.transaction_limit
{
    public class BatchUpdateFailedException : BaseException
    {
        public BatchUpdateFailedException()
        {
            statusInfo["statusCode"] = "452";
        }
    }
}

