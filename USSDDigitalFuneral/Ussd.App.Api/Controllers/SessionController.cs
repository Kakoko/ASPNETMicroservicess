using AngleDimension.Mno.DataModels;
using AngleDimension.Mno.DataModels.Contracts;
using AngleDimension.NetCore.Web.Mvc.Filters;
using AngleDimension.ServiceDiscovery.DataModels.Ussd;
using AngleDimension.Standard.Core.General;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Ussd.App.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {


        private readonly ILogger<SessionController> _logger;

        public SessionController(ILogger<SessionController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Process")]
        [HandleException("An error occurred while processing your request")]
        public async Task<IActionResult> Process([FromBody] UssdRequest request,
            [FromServices] IRequestClient<IUssdInboundRequest> client)
        {

            _logger.LogDebug("Sending request to worker {request}" , request);
            var response = await client.GetResponse<IProcessResponse>(new
            {
                UssdInbound = new UssdInboundDTO
                {
                    Body = request.Message,
                    Msisdn = request.Msisdn,
                    Sessionid = request.SessionId,
                    Type = request.SessionType
                }
            });

            var json = response.Message.ProcessResponse.Result.ToString();
            var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            var responseMessage = root.GetProperty("message").GetString();
            var sessionType = root.GetProperty("type").GetString();
            return Ok(new UssdResponse
            {
                Response = responseMessage,
                SessionType = sessionType
            });
        }
    }
}
