using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Special exceptional dates for avaiabiliy of limit.
    /// </summary>
    public class AvailabilityExceptionDefinition
    {
        /// <summary>
        /// Start of exception as CRON format.
        /// </summary>
        /// <example>
        /// 0 0 31 7 * ? 2020
        /// </example>666666
        [JsonProperty("start")]
        public string Start { get; set; }

        /// <summary>
        /// Finish of exception as CRON format.
        /// </summary>
        /// <example>
        /// 0 0 3 8 * ? 2020
        /// </example>
        [JsonProperty("finish")]
        public string Finish { get; set; }

        [JsonProperty("descriptions")]
        public AvailabilityExceptionDescriptionDefinition[] Descriptions { get; set; }
    }
}
