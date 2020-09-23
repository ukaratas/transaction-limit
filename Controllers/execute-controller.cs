using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace bbt.enterprise_library.transaction_limit.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class ExecuteController : ControllerBase
    {
        private readonly ILogger<DefinitionController> _logger;
        private readonly IBusinessService _businessService;

        public ExecuteController(ILogger<DefinitionController> logger, IBusinessService businessService)
        {
            _logger = logger;
            _businessService = businessService;
        }

        /// <summary>
        /// Executing limit with utilization or reversal purpose
        /// </summary>
        /// <remarks>
        /// Core operation of transaction limit management service. Before processing transaction just consume execute operation to limit control and limit utilization. <br /> <br />
        /// Example : <br /><br />
        /// Definitions : "withdraw/digital/38552069008/4561-1234-4561-5896", "withdraw/digital/38552069008/star", "withdraw/digital/star/star" and "withdraw/star/star/star" <br /> <br />
        /// Execute transaction for "withdraw/digital/38552069008/4561-1234-4561-5896". When an operation is performed for the this path, the same process is applied for the root paths.<br /><br />
        /// If there is a exchange code difference between the paths, the transactions are applied by performing a currency conversion. <br /><br />
        /// If type is "reversal" the opposite of normal operations.
        /// If type is "simulation" simulates transaction and return normal response, no database operations.
        /// If the execute does not find the exact path it returns 204.
        /// </remarks>
        /// <response code="200">Limit execute request is executed successfully.</response>
        /// <response code="452">Path can not be emtpy or null.</response>
        /// <response code="453">Invalid amount. Amount can not be negative. Negative is not applicable. Zero is used for rate limiting function.</response>
        /// <response code="454">"path" not enough remaining times left to process. One or more paths have zero remaining times left. So can not execute request. Could also mean there are not enough times to do reversal transactions.</response>
        /// <response code="455">"path" Minimum transaction limit:"amount". So can not execute request.</response>
        /// <response code="456">"path" Maximum transaction limit:"amount". So can not execute request.</response>
        /// <response code="457">"path" remaining amount limit not enough. One or more paths dont have enough remaining amount limit left. So can not execute request. Could also mean there are not enough limit to do reversal transactions.</response>
        /// <response code="458">Currency code is invalid. Please check the currency code.</response>
        /// <response code="459">Transaction time is not available. It could be due to exceptions such as holidays or out of available time frame. Will return closest available time. If the available time frame older than current time will return null.</response> 
        /// <response code="460">Path is not defined.</response>
        /// <response code="461">This path is parent. You can not execute on parent paths.</response>
        /// <response code="462">Exchange rates could not be obtained. Exchange rates API could be down.</response>
        /// <response code="463">Path at also-look is not defined. The path at also-look not found. If you do not want to use it send "none" or null.</response>
        /// <response code="464">An error has been occured in database.</response>
        /// <response code="500">Technical error on the system.</response>
        [HttpPost()]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(typeof(ExecuteResponseDefinition[]), 200)]
        [ProducesResponseType(typeof(LimitDefinition), 454)]
        [ProducesResponseType(typeof(LimitDefinition), 455)]
        [ProducesResponseType(typeof(LimitDefinition), 456)]
        [ProducesResponseType(typeof(LimitDefinition), 457)]
        [ProducesResponseType(typeof(AvailabilityRejectDefinition), 459)]
        public ActionResult<IEnumerable<ExecuteResponseDefinition>> Execute([FromBody] ExecuteRequestDefinition executeRequest)
        {
            try
            {
                return _businessService.ExecuteTransaction(executeRequest).ToList();
            }
            catch (Exception e)
            {
                if (e.Data["statusCode"] != null && e.Data["message"] != null) return StatusCode(Int32.Parse(e.Data["statusCode"].ToString()), e.Data["message"]);
                else return StatusCode(500, e.ToString());
            }
        }
    }
}
