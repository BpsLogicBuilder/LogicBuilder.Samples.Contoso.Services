using Contoso.Bsl.Flow.Rules;
using Contoso.Bsl.Flow.Rules.Interfaces;
using LogicBuilder.DataContracts;
using LogicBuilder.RulesDirector;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Bsl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController(IRulesCache rulesCache, IRulesLoader rulesLoader, IWebHostEnvironment webHostEnvironment) : ControllerBase
    {
        private readonly IRulesCache _rulesCache = rulesCache;
        private readonly IRulesLoader _rulesLoader = rulesLoader;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        [HttpPost("PostFileData")]
        public async Task<IActionResult> PostFileData([FromBody] ModuleData moduleData)
        {
            try
            {
                if (_webHostEnvironment.EnvironmentName != "Development")
                {
                    throw new InvalidOperationException(
                        "This shouldn't be invoked in non-development environments.");
                }

                await _rulesLoader.LoadRules
                (
                    new RulesModule
                    (
                        moduleData.ModuleName.ToLowerInvariant(),
                        moduleData.ResourcesStream,
                        moduleData.RulesStream
                    ),
                    _rulesCache
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
