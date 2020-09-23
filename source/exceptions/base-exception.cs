using System;
using System.Collections.Generic;

namespace bbt.enterprise_library.transaction_limit
{
    public class BaseException : Exception
    {
        public Dictionary<string, object> statusInfo = new Dictionary<string, object>();
        public override System.Collections.IDictionary Data
        {
            get
            {
                return statusInfo;
            }
        }
    }
}