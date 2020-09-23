namespace bbt.enterprise_library.transaction_limit
{
    public class DatabaseException : BaseException
    {
        public DatabaseException()
        {
            statusInfo["statusCode"] = "464";
            statusInfo["message"] = "An error has been occured in database.";
        }
    }
}

