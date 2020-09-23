using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Limit information of the path
    /// </summary>
    public class BatchUpdateLimitDefinition
    {
        /// <summary>
        /// New amount limit. If no changes wanted use '-1'.
        /// </summary>
        /// <example>
        /// 5000
        /// </example>
        [JsonRequired]
        [JsonProperty("amount-limit")]
        public decimal AmountLimit { get; set; }

        /// <summary>
        /// New Maximum amount limit. If no changes wanted use '-1'.
        /// </summary>
        /// <example>
        /// 15000
        /// </example>
        [JsonRequired]
        [JsonProperty("max-amount-limit")]
        public decimal MaxAmountLimit { get; set; }

        /// <summary>
        /// New default amount limit. If no changes wanted use '-1'.
        /// </summary>
        /// <example>
        /// 100
        /// </example>
        [JsonRequired]
        [JsonProperty("default-amount-limit")]
        public decimal DefaultAmountLimit { get; set; }

        /// <summary>
        /// New timer limit. If no changes wanted use '-1'.
        /// </summary>
        /// <example>
        /// 100
        /// </example>
        [JsonRequired]
        [JsonProperty("timer-limit")]
        public int TimerLimit { get; set; }

        /// <summary>
        /// New maximum timer limit. If no changes wanted use '-1'.
        /// </summary>
        /// <example>
        /// 100
        /// </example>
        [JsonRequired]
        [JsonProperty("max-timer-limit")]
        public int MaxTimerLimit { get; set; }

        /// <summary>
        /// New default timer limit. If no changes wanted use '-1'.
        /// </summary>
        /// <example>
        /// 100
        /// </example>
        [JsonRequired]
        [JsonProperty("default-timer-limit")]
        public int DefaultTimerLimit { get; set; }

    }
}
