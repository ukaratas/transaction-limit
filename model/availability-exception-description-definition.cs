using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Availability exception definition. Definition is supporting multi language support.
    /// </summary>
    public class AvailabilityExceptionDescriptionDefinition
    {
        /// <summary>
        /// Description of exception
        /// </summary>
        /// <example>
        /// National Holiday
        /// </example>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Language of the  description
        /// </summary>
        /// <example>
        /// tr
        /// </example>
        [JsonProperty("language")]
        public string Language { get; set; }
    }
}

