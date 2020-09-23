using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Duration period of the limit. Limit can be renewed at duration if set to elapsed.
    /// </summary>
    public class DurationDefinition
    {
        /// <summary>
        /// Span is duration of limit by CRON format. Format can also include predefined duration definitions such as; @daily, @weekly, @montly and @yearly.
        /// </summary>
        /// <example>
        /// @daily
        /// </example>
        [JsonProperty("span")]
        public string Span { get; set; }

        [JsonProperty("renewal")]
        public RenewalType Renewal { get; set; }
    }
}

/// <summary>
/// Renewal state of limit. If elapsed is set then when duration is elapsed limit resets automatically.
/// </summary>
/// <example>
/// elapsed
/// </example>
public enum RenewalType
{
    never = 0,
    elapsed = 1,
}

