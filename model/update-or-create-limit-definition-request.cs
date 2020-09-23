using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Limit definition is going to update
    /// </summary>
    public class UpdateOrCreateLimitDefinitionRequestDefinition
    {
        /// <summary>
        /// Path of Limit definition.
        /// </summary>
        /// <example>
        /// withdraw/burganAtm/customerNo/debitCardNo
        /// </example>
        [JsonProperty("path")]
        [JsonRequired]
        public string Path { get; set; }

        /// <summary>
        /// Minimum amount per transaction. Any new transaction amount can not be less than this amount.
        /// </summary>
        /// <example>
        /// 10
        /// </example>
        [JsonProperty("transaction-minimum-limit")]
        public decimal? TransactionMinimumLimit { get; set; }

        /// <summary>
        /// Maximum amount per transaction. Any new transaction amount can not be more than this amount.
        /// </summary>
        /// <example>
        /// 100
        /// </example>
        [JsonProperty("transaction-maximum-limit")]
        public decimal? TransactionMaximumLimit { get; set; }

        /// <summary>
        /// Maximum limit of amount. The maximum that amount limit can be.
        /// If amount limit and max amount limit does not have same type of currency bank's converter used to convert amounts. 
        /// Use '-1" when you want this field to be blank for this definition.
        /// </summary>
        /// <example>
        /// 15000
        /// </example>
        [JsonProperty("max-amount-limit")]
        public decimal? MaxAmountLimit { get; set; }

        /// <summary>
        /// Maximum amount limit's currency code.
        /// </summary>
        /// <example>
        /// TRY
        /// </example>
        [JsonProperty("max-amount-limit-currency-code")]
        public string MaxAmountLimitCurrencyCode { get; set; }

        /// <summary>
        /// Amount limit. The amount that can be used from this limit.
        /// </summary>
        /// <example>
        /// 10000
        /// </example>
        [JsonProperty("amount-limit")]
        public decimal? AmountLimit { get; set; }

        /// <summary>
        /// Maximum limit of timer. Use '-1" when you want this field to be blank for this definition.
        /// </summary>
        /// <example>
        /// 100
        /// </example>
        [JsonProperty("max-timer-limit")]
        public int? MaxTimerLimit { get; set; }

        /// <summary>
        /// Timer limit. How many times this limit can be used.
        /// </summary>
        /// <example>
        /// 11
        /// </example>
        [JsonProperty("timer-limit")]
        public int? TimerLimit { get; set; }

        /// <summary>
        /// Amount limit's currency code. Can be diffent than maximum amount limit.
        /// </summary>
        /// <example>
        /// TRY
        /// </example>
        [JsonProperty("currency-code")]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Default limit of timer. Can be used to create new limits using this value.
        /// </summary>
        /// <example>
        /// 100
        /// </example>
        [JsonProperty("default-timer-limit")]
        public int? DefaultTimerLimit { get; set; }

        /// <summary>
        /// Default amount. Can be used to create new limits using this value.
        /// </summary>
        /// <example>
        /// 100
        /// </example>
        [JsonProperty("default-amount-limit")]
        public decimal? DefaultAmountLimit { get; set; }

        /// <summary>
        /// Span is duration of limit by CRON format. Format can also include predefined duration definitions such as; @daily, @weekly, @montly and @yearly.
        /// </summary>
        /// <example>
        /// @daily
        /// </example>
        [JsonProperty("duration-span")]
        public string Span { get; set; }

        [JsonProperty("duration-renewal")]
        public RenewalType? Renewal { get; set; }

        [JsonProperty("availability")]
        public AvailabilityDefinition Availability { get; set; }

        /// <summary>
        /// This indicates of the limit is in use or not.
        /// </summary>
        /// <example>
        /// true
        /// </example>
        [JsonProperty("is-active")]
        public bool? isActive { get; set; }

        /// <summary>
        /// Path of the definition that should be checked while executing this limit.
        /// The path that is defined here will be used like a parent definition for this definition at execute method.
        /// "none" means do not check any other limit than parents.
        /// </summary>
        /// <example>
        /// none
        /// </example>
        [JsonProperty("also-look")]
        public string alsoLook { get; set; }
    }
}