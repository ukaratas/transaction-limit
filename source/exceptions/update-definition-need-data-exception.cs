namespace bbt.enterprise_library.transaction_limit
{
    public class UpdateDefinitionNeedDataException : BaseException
    {
        public UpdateDefinitionNeedDataException()
        {
            statusInfo["message"] = "Please check the field names you want to update.";
            statusInfo["statusCode"] = "464";
        }
    }
}
