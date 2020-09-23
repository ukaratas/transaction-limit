namespace bbt.enterprise_library.transaction_limit
{
    public class InvalidAmountException : BaseException
    {
        public InvalidAmountException()
        {
            statusInfo["message"] = "Invalid amount.";
            statusInfo["statusCode"] = "453";
        }
    }
}