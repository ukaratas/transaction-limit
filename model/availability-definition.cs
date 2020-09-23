using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Availability information of limit. With this configuration limit can be available like weekends, weekdays, or up to 18:00. Also special dates like holidays can be excluded from availability.
    /// </summary>
    public class AvailabilityDefinition
    {
        /// <summary>
        /// Start of limit as CRON format.
        /// </summary>
        /// <example>
        /// 0 9 * * 1-5 ? 2020
        /// </example>
        [JsonProperty("start")]
        public string Start { get; set; }

        /// <summary>
        /// Finish of limit as CRON format.
        /// </summary>
        /// <example>
        /// 0 18 * * 1-5 ? 2020
        /// </example>
        [JsonProperty("finish")]
        public string Finish { get; set; }

        /// <summary>
        /// Exception definitions.
        /// </summary>
        [JsonProperty("exceptions")]
        public AvailabilityExceptionDefinition[] Exceptions { get; set; }

        /// <example>
        /// available
        /// </example>
        [JsonIgnore]
        [JsonProperty("availability-status")]
        [JsonConverter(typeof(StringEnumConverter))]
        [EnumDataType(typeof(AvailabilityStatusType))]
        [DefaultValue(AvailabilityStatusType.NotCalculated)]
        public AvailabilityStatusType AvailabilityStatus { get; set; }
    }
}

/// <summary>
/// Availability status of limit. Using enums ; not-calculated, available, not-in-range and exception.
/// </summary>
public enum AvailabilityStatusType
{

    [EnumMember(Value = "not-calculated")]
    NotCalculated = 0,

    [EnumMember(Value = "available")]
    Available = 1,

    [EnumMember(Value = "not-in-range")]
    NotInRange = 2,

    [EnumMember(Value = "exception")]
    Exception = 3,
}

