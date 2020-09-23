using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Limit definition details 
    /// </summary>
    public class LimitDefinition
    {
        /// <summary>
        /// Path of the limit.
        /// </summary>
        /// <example>
        /// withdraw/digital/38552069008/4561-1234-4561-5896
        /// </example>
        [JsonProperty("path")]
        [StringLength(400)]
        public string Path { get; set; }

        /// <summary>
        /// Creation date of the limit. DateTime is based on server timeline.
        /// </summary>
        /// <example>
        /// 2019-11-11T14:54:41.02
        /// </example>
        [JsonProperty("created-at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Renewal date of the limit. DateTime is based on server timeline.
        /// </summary>
        /// <example>
        /// 2019-11-11T14:54:41.02
        /// </example>
        [JsonProperty("renewed-at")]
        public DateTime RenewedAt { get; set; }

        /// <summary>
        /// Maximum limit of amount. 
        /// </summary>
        /// <example>
        /// 15000
        /// </example>
        [JsonProperty("max-amount-limit")]
        public decimal? MaxAmountLimit { get; set; }

        /// <summary>
        /// Currency code of maximum amount limit. 
        /// </summary>
        /// <example>
        /// TRY
        /// </example>
        [JsonProperty("max-amount-limit-currency-code")]
        public string MaxAmountLimitCurrencyCode { get; set; }

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

        [JsonProperty("transaction-limit")]
        public TransactionLimitDefinition TransactionLimit { get; set; }

        [JsonProperty("amount-limit")]
        public AmountLimitDefinition AmountLimit { get; set; }

        [JsonProperty("timer-limit")]
        public TimerLimitDefinition TimerLimit { get; set; }

        [JsonProperty("duration")]
        public DurationDefinition Duration { get; set; }

        [JsonProperty("availability")]
        public AvailabilityDefinition Availability { get; set; }

        /// <summary>
        /// This indicates of the limit is in use or not.
        /// </summary>
        /// <example>
        /// true
        /// </example>
        [JsonProperty("is-active")]
        public bool isActive { get; set; }

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

