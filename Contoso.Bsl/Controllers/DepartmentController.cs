using Contoso.Bsl.Flow.Interfaces;
using LogicBuilder.App.Bsl.Business.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Bsl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController(IFlowManager flowManager) : ControllerBase
    {
        private readonly IFlowManager flowManager = flowManager;

        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] DeleteEntityRequest deleteDepartmentRequest)
        {
            this.flowManager.FlowDataCache.Request = deleteDepartmentRequest;
            this.flowManager.Start("deletedepartment");
            return Ok(this.flowManager.FlowDataCache.Response);
        }

        [HttpPost("Save")]
        public IActionResult Save([FromBody] SaveEntityRequest saveDepartmentRequest)
        {
            this.flowManager.FlowDataCache.Request = saveDepartmentRequest;
            this.flowManager.Start("savedepartment");
            return Ok(this.flowManager.FlowDataCache.Response);
        }
    }
}
