using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Per transaction limit information
    /// </summary>
    public class TransactionLimitDefinition
    {
        /// <summary>
        /// Minimum limit for each transaction. Any new transaction amount can not be less than this amount.
        /// </summary>
        /// <example>
        /// 5
        /// </example>
        [JsonRequired]
        [JsonProperty("minimum-limit")]
        public decimal MinimumLimit { get; set; }

        /// <summary>
        /// Maximum limit for each transaction. Any new transaction amount can not be more than this amount.
        /// </summary>
        /// <example>
        /// 1000
        /// </example>
        [JsonRequired]
        [JsonProperty("maximum-limit")]
        public decimal MaximumLimit { get; set; }

        /// <summary>
        /// Limit's currency code.
        /// </summary>
        /// <example>
        /// TRY
        /// </example>
        [JsonRequired]
        [JsonProperty("currency-code")]
        public string CurrencyCode { get; set; }
    }
}