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
    public class AnonymousTypeListController(IHttpClientHelper httpClientHelper, IOptions<UrlOptions> optionsAccessor) : ControllerBase
    {
        private readonly IHttpClientHelper _httpClientHelper = httpClientHelper;
        private readonly UrlOptions urlOptions = optionsAccessor.Value;

        [HttpPost("GetList")]
        public Task<BaseResponse> GetList([FromBody] GetObjectListRequest request)
            => _httpClientHelper.PostAsync<BaseResponse>
            (
                $"{urlOptions.BaseBslUrl}api/AnonymousTypeList/GetList",
                JsonSerializer.Serialize(request),
                SerializationOptions.Default,
                HttpClientOptions.BslClientName
            );
    }
}
