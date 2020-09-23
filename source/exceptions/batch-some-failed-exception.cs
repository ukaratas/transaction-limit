using System.Collections.Generic;

namespace bbt.enterprise_library.transaction_limit
{
    public class BatchSomeFailedException : BaseException
    {
        public BatchSomeFailedException(List<ErrorDefinition> data)
        {
            statusInfo["statusCode"] = "453";
            statusInfo["message"] = data;
        }
    }
}

