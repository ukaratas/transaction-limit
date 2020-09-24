namespace bbt.enterprise_library.transaction_limit
{
    public class CronFormatException : BaseException
    {
        public CronFormatException()
        {
            statusInfo["message"] = @"Please check the CRON fields.";
            statusInfo["statusCode"] = @"461";
        }
    }
}
