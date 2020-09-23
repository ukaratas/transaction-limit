using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    /// <summary>
    /// Special exceptional dates for availability of limit.
    /// </summary>
    public class AvailabilityRejectDefinition
    {

        /// <summary>
        /// Closest time that is available for transactions. DateTime is based on server timeline.
        /// </summary>
        /// <example>
        /// 09:00 09.04.2020
        /// </example>
        [JsonProperty("FirstAvailableDate")]
        public DateTime? FirstAvailableDate { get; set; }

        /// <summary>
        /// Reason for exception. Default is Time is not in available hours.
        /// </summary>
        /// <example>
        /// Outside of defined available hours
        /// </example>
        [JsonProperty("reason")]
        public IEnumerable<AvailabilityExceptionDescriptionDefinition> Reason { get; set; }
    }
}
