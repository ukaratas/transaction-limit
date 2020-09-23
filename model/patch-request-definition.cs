using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    public class PatchRequestDefinition
    {
        /// <summary>
        /// Utilized amount limit to be updated. '-1' when wanted to keep unchanged.
        /// </summary>
        /// <example>
        /// 5000
        /// </example>
        [JsonRequired]
        [JsonProperty("new-amount-utilized-limit")]
        public decimal NewAmountUtilizedLimit { get; set; }

        /// <summary>
        /// Utilized timer limit to be updated. '-1' when wanted to keep unchanged.
        /// </summary>
        /// <example>
        /// 15
        /// </example>
        [JsonRequired]
        [JsonProperty("new-timer-utilized-limit")]
        public decimal NewTimerUtilizedLimit { get; set; }
    }
}
