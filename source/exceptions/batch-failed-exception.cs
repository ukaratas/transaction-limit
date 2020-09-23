using System.Collections.Generic;

namespace bbt.enterprise_library.transaction_limit
{
    public class BatchFailedException : BaseException
    {
        public BatchFailedException(List<ErrorDefinition> data)
        {
            statusInfo["statusCode"] = "452";
            statusInfo["message"] = data;
        }
    }
}

