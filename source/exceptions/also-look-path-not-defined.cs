namespace bbt.enterprise_library.transaction_limit
{
    public class AlsoLookPathNotDefined : BaseException
    {
        public AlsoLookPathNotDefined()
        {
            statusInfo["message"] = "Path at also-look not defined.";
            statusInfo["statusCode"] = "463";
        }
    }
}