using LogicBuilder.App.Bsl.Business.Requests;
using LogicBuilder.App.Bsl.Business.Responses;
using LogicBuilder.App.Web.Utils.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;

namespace Contoso.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController(IHttpClientHelper httpClientHelper, IOptions<UrlOptions> optionsAccessor) : ControllerBase
    {
        private readonly IHttpClientHelper _httpClientHelper = httpClientHelper;
        private readonly UrlOptions urlOptions = optionsAccessor.Value;

        [HttpPost("Delete")]
        public Task<BaseResponse> Delete([FromBody] DeleteEntityRequest deleteDepartmentRequest)
            => _httpClientHelper.PostAsync<BaseResponse>
            (
                $"{urlOptions.BaseBslUrl}api/Department/Delete",
                JsonSerializer.Serialize(deleteDepartmentRequest),
                SerializationOptions.Default
            );

        [HttpPost("Save")]
        public Task<BaseResponse> Save([FromBody] SaveEntityRequest saveDepartmentRequest)
            => _httpClientHelper.PostAsync<BaseResponse>
            (
                $"{urlOptions.BaseBslUrl}api/Department/Save",
                JsonSerializer.Serialize(saveDepartmentRequest),
                SerializationOptions.Default
            );
    }
}
