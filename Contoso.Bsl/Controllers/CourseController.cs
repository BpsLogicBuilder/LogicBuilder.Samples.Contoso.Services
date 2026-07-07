using Contoso.Bsl.Flow.Interfaces;
using LogicBuilder.App.Bsl.Business.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Bsl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController(IFlowManager flowManager) : ControllerBase
    {
        private readonly IFlowManager flowManager = flowManager;

        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] DeleteEntityRequest deleteCourseRequest)
        {
            this.flowManager.FlowDataCache.Request = deleteCourseRequest;
            this.flowManager.Start("deletecourse");
            return Ok(this.flowManager.FlowDataCache.Response);
        }

        [HttpPost("Save")]
        public IActionResult Save([FromBody] SaveEntityRequest saveCourseRequest)
        {
            this.flowManager.FlowDataCache.Request = saveCourseRequest;
            this.flowManager.Start("savecourse");
            return Ok(this.flowManager.FlowDataCache.Response);
        }
    }
}
