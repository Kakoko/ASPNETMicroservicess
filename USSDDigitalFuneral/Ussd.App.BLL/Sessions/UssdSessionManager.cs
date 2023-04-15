using AngleDimension.NetCore.Ussd.SessionManagement;
using StackExchange.Redis.Extensions.Core.Abstractions;


namespace Ussd.App.BLL.Sessions
{
    public class UssdSessionManager : SessionManagerBase<LanguageDto>
    {
        public UssdSessionManager(IRedisClient client) : base(client)
        {
        }
    }
}