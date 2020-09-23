using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Special exceptional dates for avaiabiliy of limit.
    /// </summary>
    public class ErrorDefinition
    {
        public string Path { get; set; }
        public string Code { get; set; }
    }
}
