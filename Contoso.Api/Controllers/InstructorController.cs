using LogicBuilder.App.Bsl.Business.Requests;
using LogicBuilder.App.Bsl.Business.Responses;
using LogicBuilder.App.Utils.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;

namespace Contoso.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController(IHttpClientHelper httpClientHelper, IOptions<UrlOptions> optionsAccessor) : ControllerBase
    {
        private readonly IHttpClientHelper _httpClientHelper = httpClientHelper;
        private readonly UrlOptions urlOptions = optionsAccessor.Value;

        [HttpPost("Delete")]
        public Task<BaseResponse> Delete([FromBody] DeleteEntityRequest deleteInstructorRequest)
            => _httpClientHelper.PostAsync<BaseResponse>
            (
                $"{urlOptions.BaseBslUrl}api/Instructor/Delete",
                JsonSerializer.Serialize(deleteInstructorRequest),
                SerializationOptions.Default,
                HttpClientOptions.BslClientName
            );

        [HttpPost("Save")]
        public Task<BaseResponse> Save([FromBody] SaveEntityRequest saveInstructorRequest)
            => _httpClientHelper.PostAsync<BaseResponse>
            (
                $"{urlOptions.BaseBslUrl}api/Instructor/Save",
                JsonSerializer.Serialize(saveInstructorRequest),
                SerializationOptions.Default,
                HttpClientOptions.BslClientName
            );
    }
}
