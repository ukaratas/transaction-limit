namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidSpanException : BaseException
    {
        public InvalidSpanException()
        {
            statusInfo["message"] = "Span type is invalid.";
            statusInfo["statusCode"] = "459";
        }
    }
}
