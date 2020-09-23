using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace bbt.enterprise_library.transaction_limit.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<DefinitionController> _logger;
        private readonly IBusinessService _businessService;

        public SearchController(ILogger<DefinitionController> logger, IBusinessService businessService)
        {
            _logger = logger;
            _businessService = businessService;
        }

        /// <summary>
        /// Returns queried limit definitions.
        /// </summary>
        /// <remarks>
        /// Querying limit definitions by paths. <br /> <br />
        /// Path example 1: "withdraw/~/~/4561-1234-4561-5896" to get all paths that end with "4561-1234-4561-5896". <br /> <br />
        /// Path example 2 "withdraw/digital/38552069008/%" to get all paths under "withdraw/digital/38552069008/". <br /> <br />
        /// % or ~ can be used in path to tell any from that path. It works like SQL Like.<br /> <br />
        /// PageIndex needs to be zero(0) to see first page of search. <br /> <br />
        /// PageSize can make the query faster to run if used efficiently.<br /> <br />
        /// </remarks>
        /// <param name="query">Query to search paths.</param>
        /// <param name="isActive">True means only active definitions will be searched. False means all definitions regardsless of active will be searched.</param>
        /// <param name="PageIndex">Number of page to load. Use zero(0) to load first page</param>
        /// <param name="pageSize">Size of each page.</param>
        /// <response code="200">All query matching limit definitions have returned successfully.</response>        
        /// <response code="204">Query result does not have any results to show.</response>
        /// <response code="452">Query can not be empty. Please send the query.</response>
        /// <response code="453">Page Size can not be zero or negative. Page Size can not be higher than 1000.</response>
        /// <response code="454">Page Index can not be negative. </response>
        /// <response code="500">Technical error on the system.</response>
        [Route("{page-index}/{page-size}")]
        [HttpGet()]
        public ActionResult<SearchDefinitionsResponseDefinition> Search(string query, [FromRoute(Name = "page-index")] int PageIndex, [FromRoute(Name = "page-size")] int pageSize, [FromQuery(Name = "is-active"), Required] bool isActive)
        {
            try
            {
                var result = _businessService.SearchDefinitions(query, PageIndex, pageSize, isActive);
                if (!result.LimitDefinitions.Any()) return StatusCode(204);
                else return result;
            }
            catch (Exception e)
            {
                if (e.Data["statusCode"] != null && e.Data["message"] != null) return StatusCode(Int32.Parse(e.Data["statusCode"].ToString()), e.Data["message"]);
                else if (e.Data["statusCode"] != null) return StatusCode(Int32.Parse(e.Data["statusCode"].ToString()));
                else return StatusCode(500, "Technical error on the system.");
            }
        }

    }
}

