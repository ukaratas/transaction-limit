using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Timer limit information of the path
    /// </summary>
    public class TimerLimitDefinition
    {
        /// <summary>
        /// Maximum limit of timer. 
        /// </summary>
        /// <example>
        /// 100
        /// </example>
        [JsonProperty("max-timer-limit")]
        public int MaxTimerLimit { get; set; }

        /// <summary>
        /// Timer limit of path. How many times this path can be used.
        /// </summary>
        /// <example>
        /// 10
        /// </example>
        [JsonRequired]
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Utilized times of limit. How many times the limit has been used.
        /// </summary>
        /// <example>
        /// 4
        /// </example>
        [JsonRequired]
        [JsonProperty("utilized")]
        public int Utilized { get; set; }

        /// <summary>
        /// Remaining times of limit. How many times more the limit can be used.
        /// </summary>
        /// <example>
        /// 6
        /// </example>
        [JsonRequired]
        [JsonProperty("remaining")]
        public int Remaining { get; set; }
    }
}

