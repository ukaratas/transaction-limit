using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Message body of the execution operation 
    /// </summary>
    public class ExecuteResponseDefinition
    {
        /// <summary>
        /// Path of the limit 
        /// </summary>
        /// <example>
        /// withdraw/digital/38552069008/4561-1234-4561-5896
        /// </example>
        [JsonRequired]
        [JsonProperty("path")]
        [StringLength(400)]
        public string Path { get; set; }

        /// <summary>
        /// Span is duration of limit by CRON format. Format can also include predefined duration definitions such as; @daily, @weekly, @montly and @yearly.
        /// </summary>
        /// <example>
        /// @daily
        /// </example>
        [JsonProperty("span")]
        [StringLength(400)]
        public string Duration { get; set; }

        [JsonProperty("transaction-limit")]
        public TransactionLimitDefinition TransactionLimit { get; set; }

        /// <summary>
        /// Total amount limit information 
        /// </summary>
        [JsonProperty("amount-limit")]
        public AmountLimitDefinition AmountLimit { get; set; }

        /// <summary>
        /// Total timer limit information 
        /// </summary>
        [JsonProperty("timer-limit")]
        public TimerLimitDefinition TimerLimit { get; set; }
    }
}

