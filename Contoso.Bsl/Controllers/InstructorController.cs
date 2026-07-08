using Contoso.Bsl.Flow.Interfaces;
using LogicBuilder.App.Bsl.Business.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Bsl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController(IFlowManager flowManager) : ControllerBase
    {
        private readonly IFlowManager flowManager = flowManager;

        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] DeleteEntityRequest deleteInstructorRequest)
        {
            this.flowManager.FlowDataCache.Request = deleteInstructorRequest;
            this.flowManager.Start("deleteinstructor");
            return Ok(this.flowManager.FlowDataCache.Response);
        }

        [HttpPost("Save")]
        public IActionResult Save([FromBody] SaveEntityRequest saveInstructorRequest)
        {
            this.flowManager.FlowDataCache.Request = saveInstructorRequest;
            this.flowManager.Start("saveinstructor");
            return Ok(this.flowManager.FlowDataCache.Response);
        }
    }
}
