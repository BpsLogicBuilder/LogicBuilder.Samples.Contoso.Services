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
    public class StudentController(IHttpClientHelper httpClientHelper, IOptions<UrlOptions> optionsAccessor) : ControllerBase
    {
        private readonly IHttpClientHelper _httpClientHelper = httpClientHelper;
        private readonly UrlOptions urlOptions = optionsAccessor.Value;

        [HttpPost("Delete")]
        public Task<BaseResponse> Delete([FromBody] DeleteEntityRequest deleteStudentRequest)
            => _httpClientHelper.PostAsync<BaseResponse>
            (
                $"{urlOptions.BaseBslUrl}api/Student/Delete",
                JsonSerializer.Serialize(deleteStudentRequest),
                SerializationOptions.Default,
                HttpClientOptions.BslClientName
            );

        [HttpPost("Save")]
        public Task<BaseResponse> Save([FromBody] SaveEntityRequest saveStudentRequest)
            => _httpClientHelper.PostAsync<BaseResponse>
            (
                $"{urlOptions.BaseBslUrl}api/Student/Save",
                JsonSerializer.Serialize(saveStudentRequest),
                SerializationOptions.Default,
                HttpClientOptions.BslClientName
            );
    }
}
