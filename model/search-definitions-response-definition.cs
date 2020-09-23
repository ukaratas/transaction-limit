using System.Collections.Generic;
using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    public class SearchDefinitionsResponseDefinition
    {
        /// <summary>
        /// Definitions list has more page. True/False.
        /// </summary>
        [JsonProperty("has-next-page")]
        public bool HasNextPage { get; set; }

        /// <summary>
        /// Limit Definitions Array
        /// </summary>
        [JsonProperty("limit-definitions")]
        public IEnumerable<LimitDefinition> LimitDefinitions { get; set; }
    }
}