using System.Collections.Generic;

namespace bbt.enterprise_library.transaction_limit
{
    public class NewDefinitionNeedDataException : BaseException
    {
        public NewDefinitionNeedDataException(List<string> data)
        {
            statusInfo["message"] = "Please fill in the empty fields; " + string.Join(", ", data.ToArray()) + ".";
            statusInfo["statusCode"] = "460";
        }
    }
}


