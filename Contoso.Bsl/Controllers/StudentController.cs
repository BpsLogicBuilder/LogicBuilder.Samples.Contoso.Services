using Contoso.Bsl.Flow.Interfaces;
using LogicBuilder.App.Bsl.Business.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Bsl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(IFlowManager flowManager) : ControllerBase
    {
        private readonly IFlowManager flowManager = flowManager;

        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] DeleteEntityRequest deleteStudentRequest)
        {
            this.flowManager.FlowDataCache.Request = deleteStudentRequest;
            this.flowManager.Start("deletestudent");
            return Ok(this.flowManager.FlowDataCache.Response);
        }

        [HttpPost("Save")]
        public IActionResult Save([FromBody] SaveEntityRequest saveStudentRequest)
        {
            this.flowManager.FlowDataCache.Request = saveStudentRequest;
            this.flowManager.Start("savestudent");
            return Ok(this.flowManager.FlowDataCache.Response);
        }
    }
}
