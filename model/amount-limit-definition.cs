using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Limit information of the path
    /// </summary>
    public class AmountLimitDefinition
    {
        /// <summary>
        /// Limit of path with given currency code
        /// </summary>
        /// <example>
        /// 5000
        /// </example>
        [JsonRequired]
        [JsonProperty("limit")]
        public decimal Limit { get; set; }

        /// <summary>
        /// Utilized amount of limit 
        /// </summary>
        /// <example>
        /// 1000
        /// </example>
        [JsonRequired]
        [JsonProperty("utilized")]
        public decimal Utilized { get; set; }

        /// <summary>
        /// Remaining amount of limit 
        /// </summary>
        /// <example>
        /// 4000
        /// </example>
        [JsonRequired]
        [JsonProperty("remaining")]
        public decimal Remaining { get; set; }

        /// <summary>
        /// Currency code of limit 
        /// </summary>
        /// <example>
        /// TRY
        /// </example>
        [JsonRequired]
        [JsonProperty("currency-code")]
        public string CurrencyCode { get; set; }
    }
}
