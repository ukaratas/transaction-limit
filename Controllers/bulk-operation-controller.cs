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
    public class BulkOperationController : ControllerBase
    {
        private readonly ILogger<DefinitionController> _logger;
        private readonly IBusinessService _businessService;

        public BulkOperationController(ILogger<DefinitionController> logger, IBusinessService businessService)
        {
            _logger = logger;
            _businessService = businessService;
        }

        /// <summary>
        /// Updates all paths with given values where the query is matched.
        /// </summary>
        /// <remarks>
        /// Updating all definitions that match the query. <br /> <br />
        /// Update values '-2' can be used on each value to skip updating that parameter.<br /> <br />
        /// Query values '-2' can be used on each value to skip querying for that parameter.<br /> <br />
        /// </remarks>
        /// <param name="queryPattern">Query pattern to update paths.</param>
        /// <param name="duration">Durations from the queried paths to be updated. Use '-2' to select all durations.</param>
        /// <param name="amountLimit">Amount limit from the queried paths to be updated. Use '-2' to ignore amount limit on query.</param>
        /// <param name="currencyCode">Currency Code from the queried paths to be updated.</param>
        /// <param name="timerLimit">Timer limit from the queried paths to be updated. Use '-2' to ignore timer limit on query.</param>
        /// <param name="newLimits">Update values '-2' can be used on each value to skip updating that parameter.</param>
        /// <response code="200">Update worked successfully.</response> 
        /// <response code="452">Update has failed.</response> 
        /// <response code="453">Some changes failed to update.Valide the update or try again.</response> 
        /// <response code="454">Invalid timer limit. Cannot be less than 0 '-1' is an exception.</response>
        /// <response code="458">Currency code is invalid.</response>
        /// <response code="465">Invalid maximum amount limit. Can not be negative. '-1' is an exception.</response>
        /// <response code="468">Invalid amount limit. Cannot be less than 0 '-1' is an exception.</response>
        /// <response code="470">Timer limit can't be bigger than maximum timer limit.</response>
        /// <response code="471">Invalid maximum timer limit. Can not be negative. '-1' is an exception.</response>
        /// <response code="472">Invalid default amount limit. Can not be negative. '-1' is an exception.</response>
        /// <response code="473">Invalid default timer limit. Can not be negative. '-1' is an exception.</response>
        /// <response code="500">Technical error on the system.</response>
        [Route("Update")]
        [HttpPost()]
        public ActionResult Update([Required] string queryPattern, [Required] string duration, [Required] int timerLimit, [Required] decimal amountLimit, [Required] string currencyCode, [Required] BatchUpdateLimitDefinition newLimits)
        {
            try
            {
                byte result = _businessService.updateBatch(queryPattern, duration, timerLimit, amountLimit, currencyCode, newLimits);
                if (result == 0) return StatusCode(200);
                else if (result == 1) return StatusCode(453, "Some changed failed to update.Valide the update or try again.");
                else return StatusCode(500, "Technical error on the system.");

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

