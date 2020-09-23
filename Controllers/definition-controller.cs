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
    public class DefinitionController : ControllerBase
    {
        private readonly ILogger<DefinitionController> _logger;
        private readonly IBusinessService _businessService;

        public DefinitionController(ILogger<DefinitionController> logger, IBusinessService businessService)
        {
            _logger = logger;
            _businessService = businessService;
        }

        /// <summary>
        /// Returns limit definition by path. Can include root definitions.
        /// </summary>
        /// <remarks>
        /// Get limit definitions. <br /> <br />
        /// Path example : "withdraw/digital/38552069008/4561-1234-4561-5896" <br /> <br />
        /// If "includeVariants = true" and if root paths is exist, returns definition with root definitions. <br /> <br />
        /// For example: "withdraw/digital/38552069008/4561-1234-4561-5896", "withdraw/digital/38552069008/star", "withdraw/digital/star/star" and "withdraw/star/star/star" <br /> <br />
        /// Not every path has to be a root path, for example it can only be itself: "limitsofspecialcustomer" etc. <br /> <br />
        /// IMPORTANT WARNING : star = * (because of the swagger)
        /// </remarks>
        /// <param name="path">Path is going to query.</param>
        /// <param name="includeVariants">Does query has to include starred variants of path ? </param>
        /// <response code="200">All matched limit definitions have returned successfully.</response>
        /// <response code="460">Request path is not found. No limit definitions found on this path.</response>
        /// <response code="500">Technical error on the system.</response>
        [HttpGet()]
        [ProducesResponseType(typeof(LimitDefinition[]), 200)]
        public ActionResult<IEnumerable<LimitDefinition>> Get([FromQuery, Required] string path, [FromQuery] bool includeVariants = false)
        {
            try
            {
                return _businessService.GetDefinitions(path, includeVariants).ToList();
            }
            catch (Exception e)
            {
                if (e.Data["statusCode"] != null && e.Data["message"] != null) return StatusCode(Int32.Parse(e.Data["statusCode"].ToString()), e.Data["message"]);
                else return StatusCode(500, "Technical error on the system.");
            }
        }

        /// <summary>
        /// Creates a new limit definition or updates existing one.
        /// </summary>
        /// <remarks>
        /// Definition examples (just an example, it doesn't have to be like this): <br /> <br /> 
        /// For example create atm limits with root definition: <br /> <br /> 
        /// "withdraw/burganAtm/customerNo/debitCardNo" -> This path shows the customer withdrawal limits from Burgan atm with specific debit card. <br /> <br /> 
        /// "withdraw/isbankAtm/customerNo/debitCardNo" -> This path shows the customer withdrawal limits from Is Bank atm with specific debit card. <br /> <br /> 
        /// "withdraw/burganAtm/customerNo/star" -> This path shows the customer total limits it can draw with a debit card from Burgan atm. <br /> <br /> 
        /// "withdraw/burganAtm/star/star" -> This path shows the total limits withdrawal from Burgan atm for all customers. <br /> <br /> 
        /// "withdraw/star/star/star"-> This path shows the total withdrawal limits across the bank. <br /> <br /> 
        /// Not every path has to be a root path, for example it can only be itself: "limitsofspecialcustomer" etc. <br /> <br />
        /// amount-limit, timer-limit, max-amount-limit and max-timer-limit can be set to '-1' to make them insignificant on that level.<br /> <br />
        /// With this limit will be ignored on this level and get checked at the parent definition. Should not be used with definitions without parent paths.<br /> <br />
        /// Availability -> start and finish(not in exceptions fields) cron fields, if you want to do unlimited time during the day (only standart for unlimited definitions): <br /> <br />
        /// unlimited examples : start : "* * * * 1-5" finish : "* * * * 1-5" -> this definition unlimited on weekdays, if 1-5 -> 0-6 unlimited on week. <br /> <br />
        /// normal cron example : start : "* 9 * * 1-5" finish : "* 18 * * 1-5" -> transaction time in between 9-18 on weekdays. <br /> <br />
        /// normal cron example with year : start : "* 9 * * 1-5 ? 2020" finish : "* 18 * * 1-5 ? 2020" -> transaction time in between 9-18 on weekdays at year 2020. <br /> <br />
        /// unlimited examples with year: start : "* * * * 1-5 ? 2020" finish : "* * * * 1-5 ? 2020" -> this definition unlimited on weekdays, if 1-5 -> 0-6 unlimited on week at year 2020. <br /> <br />
        /// IMPORTANT WARNING : star = * (because of the swagger)
        /// </remarks>
        /// <param name="data">Data to create or update definitions.</param>
        /// <response code="200">Limit definition has been created/updated succesfully.</response>
        /// <response code="452">Please send the path. Path can not be empty.</response>
        /// <response code="453">Please check the minimum and maximum limit.</response>
        /// <response code="454">Invalid timer limit. Cannot be less than 0 '-1' is an exception</response>
        /// <response code="455">Please check the maximum limit.</response>
        /// <response code="456">Please check the minimum limit.</response>
        /// <response code="458">Currency code is invalid.</response>
        /// <response code="459">Span type is invalid.</response>
        /// <response code="460">Please fill in the empty fields.</response>
        /// <response code="461">Please check the CRON fields.</response>
        /// <response code="462">Exchange rates could not be obtained. Exchange rates API could be down.</response>
        /// <response code="463">Please check the exception dates.</response>
        /// <response code="464">Please check the field names you want to update.</response>
        /// <response code="465">Invalid maximum amount limit. Can not be negative. '-1' is an exception.</response>
        /// <response code="466">Amount limit can't be bigger than maximum amount limit.</response>
        /// <response code="467">Maximum amount limit currency code is invalid.</response>
        /// <response code="468">Invalid amount limit.</response>
        /// <response code="469">Invalid path format.</response>
        /// <response code="470">Timer limit can't be bigger than maximum timer limit.</response>
        /// <response code="471">Invalid maximum timer limit. Can not be negative. '-1' is an exception.</response>
        /// <response code="472">Invalid default amount limit. Can not be negative. '-1' is an exception.</response>
        /// <response code="473">Invalid default timer limit. Can not be negative. '-1' is an exception.</response>
        /// <response code="500">Technical error on the system.</response>
        [HttpPost()]
        public ActionResult Post([FromBody] UpdateOrCreateLimitDefinitionRequestDefinition data)
        {
            try
            {
                _businessService.UpdateDefinition(data);
                return StatusCode(200);
            }
            catch (Exception e)
            {
                if (e.Data["statusCode"] != null && e.Data["message"] != null) return StatusCode(Int32.Parse(e.Data["statusCode"].ToString()), e.Data["message"]);
                else if (e.Data["statusCode"] != null) return StatusCode(Int32.Parse(e.Data["statusCode"].ToString()));
                else return StatusCode(500, "Technical error on the system.");
            }
        }

        /// <summary>
        /// Creates a batch of new limit definitions or updates existing ones. Can also update and create in same batch.
        /// </summary>
        /// <remarks>
        /// Can take multiple definitions at same time below is an example for a single definition.<br /> <br />
        /// Definition examples (just an example, it doesn't have to be like this): <br /> <br /> 
        /// For example create atm limits with root definition: <br /> <br /> 
        /// "withdraw/burganAtm/customerNo/debitCardNo" -> This path shows the customer withdrawal limits from Burgan atm with specific debit card. <br /> <br /> 
        /// "withdraw/isbankAtm/customerNo/debitCardNo" -> This path shows the customer withdrawal limits from Is Bank atm with specific debit card. <br /> <br /> 
        /// "withdraw/burganAtm/customerNo/star" -> This path shows the customer total limits it can draw with a debit card from Burgan atm. <br /> <br /> 
        /// "withdraw/burganAtm/star/star" -> This path shows the total limits withdrawal from Burgan atm for all customers. <br /> <br /> 
        /// "withdraw/star/star/star"-> This path shows the total withdrawal limits across the bank. <br /> <br /> 
        /// Not every path has to be a root path, for example it can only be itself: "limitsofspecialcustomer" etc. <br /> <br />
        /// Availability -> start and finish(not in exceptions fields) cron fields, if you want to do unlimited time during the day (only standart for unlimited definitions): <br /> <br />
        /// unlimited examples : start : "* * * * 1-5" finish : "* * * * 1-5" -> this definition unlimited on weekdays, if 1-5 -> 0-6 unlimited on week. <br /> <br />
        /// normal cron example : start : "* 9 * * 1-5" finish : "* 18 * * 1-5" -> transaction time in between 9-18 on weekdays. <br /> <br />
        /// IMPORTANT WARNING : star = * (because of the swagger)
        /// </remarks>
        /// <param name="datas">Data to create or update definitions. An array of data to upadate or create a batch of definitions. Can contain update definitions and create definitions at same batch.</param>
        /// <response code="200">Batch of definitions created/updated all has been completed succesfully.</response>
        /// <response code="452">All definitions failed to create/update. Check the response body for details on errors.</response>
        /// <response code="453">Some definitions failed to create/update. Check the response body for details on errors.</response>
        /// <response code="500">Technical error on the system.</response>
        [HttpPost()]
        [Route("Batch")]
        public ActionResult<List<ErrorDefinition>> BatchPost([FromBody] UpdateOrCreateLimitDefinitionRequestDefinition[] datas)
        {
            try
            {
                _businessService.BatchUpdateDefinition(datas);
                return StatusCode(200);
            }
            catch (Exception e)
            {
                if (e.Data["statusCode"] != null && e.Data["message"] != null) return StatusCode(Int32.Parse(e.Data["statusCode"].ToString()), e.Data["message"]);
                else return StatusCode(500);
            }
        }

        /// <summary>
        /// Patches the limit definition with new-utilized-amount-limit and/or new-utilized-timer-limit.
        /// </summary>
        /// <remarks>
        /// Takes definition's path and duration to update limit definition.<br /> <br /> 
        /// new-utilized-amount-limit and new-utilized-timer-limit are optional can be updated seperatly or at same time.<br /> <br />
        /// Use '-1' when you do not want to update.<br /> <br />
        /// </remarks>
        /// <param name="path">Which path to be updated.</param>
        /// <param name="duration">Which duration of the path to be updated.</param>
        /// <param name="data">Data to update definitions. Use '-1' to skip that param.</param>
        /// <response code="200">Limit definition updated succesfully.</response>
        /// <response code="453">Amount limit can not be negative. Negative is not applicable.</response>
        /// <response code="454">Timer limit can not be negative.</response>
        /// <response code="460">Request path is not found. No limit definitions found with this path and duration.</response>
        [HttpPut]
        public ActionResult<LimitDefinition> Put([FromQuery, Required] string path, [FromQuery, Required] string duration, [FromBody] PatchRequestDefinition data)
        {
            try
            {
                return _businessService.PatchDefinition(path, duration, data);
            }
            catch (Exception e)
            {
                if (e.Data["statusCode"] != null && e.Data["message"] != null) return StatusCode(Int32.Parse(e.Data["statusCode"].ToString()), e.Data["message"]);
                else return StatusCode(500);
            }
        }

    }
}

