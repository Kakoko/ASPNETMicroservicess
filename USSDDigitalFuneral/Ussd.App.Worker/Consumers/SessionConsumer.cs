using AngleDimension.Mno.DataModels;
using AngleDimension.Mno.DataModels.Contracts;
using AngleDimension.NetCore.Ussd.Abstractions;
using AngleDimension.Standard.Core.General;
using MassTransit;
using System.Threading.Tasks;

namespace Ussd.App.Worker.Consumers
{
    public class SessionConsumer : IConsumer<IUssdInboundRequest>
    {
        private readonly IUssdRequestService _service;

        public SessionConsumer(IUssdRequestService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<IUssdInboundRequest> context)
        {
            var request = context.Message.UssdInbound;
            var response = await _service.GetResponse(new UssdRequestDTO
            {
                Message = request.Body,
                Msisdn = request.Msisdn,
                SessionId = request.Sessionid,
                Type = request.Type
            });
            await context.RespondAsync<IProcessResponse>(new
            {
                ProcessResponse = new ProcessResponse
                {
                    IsErrorOccurred = false,
                    Result = response
                }
            });
        }
    }
}
