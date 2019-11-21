using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IOEdiProcessor.Data.Context;
using IOEdiProcessor.Logic;
using Microsoft.AspNetCore.Mvc;

namespace IOEdiProcessor.Api.Controllers
{
    /// <summary>
    /// Handles all molds API calls
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class EdifactController : ControllerBase
    {
        private readonly IOEdiProcessorContext _context;

        /// <summary>
        /// Controller.
        /// </summary>
        /// <param name="context"></param>
        public EdifactController(IOEdiProcessorContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet("Edi")]
        public async Task<IActionResult> GenerateEDIFile(string shipPlanId, string custID, string docType, string PurposeCode)
        {
            EdifactLogic logic = new EdifactLogic(_context);
            return Ok(await logic.GenerateEDIFile(shipPlanId, custID, docType, PurposeCode));
        }
    }
}
