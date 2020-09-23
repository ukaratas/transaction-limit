using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Message body of the execution operation 
    /// </summary>
    public class ExecuteRequestDefinition
    {
        /// <summary>
        /// Path of the limit to be executed.
        /// </summary>
        /// <example>
        /// withdraw/digital/38552069008/4561-1234-4561-5896
        /// </example>
        [JsonRequired]
        [JsonProperty("path")]
        [StringLength(400)]
        public string Path { get; set; }

        /// <summary>
        /// The amount to be executed. 
        /// </summary>
        /// <example>
        /// 100
        /// </example>
        [JsonProperty("amount")]
        [JsonRequired]
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code of amount. If currency code is equal to limit definition's currency code then it is directly executed. If it is different then before execution converted to limit currency with bank's exchange rates.
        /// </summary>
        /// <example>
        /// TRY
        /// </example>
        [JsonProperty("currency-code")]
        [JsonRequired]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Type of execute ; reversal, simulation or utilize. Explains what execute command will do.
        /// </summary>
        /// <example>
        /// utilize
        /// </example>
        [JsonProperty("type")]
        [JsonRequired]
        [JsonConverter(typeof(StringEnumConverter))]
        [EnumDataType(typeof(ExecutionType))]
        public ExecutionType Type { get; set; }

        /// <summary>
        /// If the exact path is required while executing transaction. Default value is true. When this is false it enables the usage of limit definitions that are not yet defined as full path but has been defined on a higher levels.
        /// </summary>
        /// <example>
        /// true
        /// </example>
        [JsonProperty("is-exact-path-required")]
        [JsonRequired]
        public bool isExactPathRequired { get; set; }
    }
}

/// <summary>
/// Type of execution. If process is utilization set the type to utilize, if the process refund, reversal or cancelation set to reversal.
/// </summary>
public enum ExecutionType
{
    [EnumMember(Value = "utilize")]
    Utilize = 1,

    [EnumMember(Value = "reversal")]
    Reversal = 2,

    [EnumMember(Value = "simulation")]
    Simulation = 3
}