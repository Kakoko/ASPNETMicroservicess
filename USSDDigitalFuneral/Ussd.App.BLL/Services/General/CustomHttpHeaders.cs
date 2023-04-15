using AngleDimension.Standard.Http.HttpServices;

namespace Ussd.App.BLL.Services.Genenal
{
    public static class CustomHttpHeaders
    {
        public static List<HttpCustomHeaderField> Set(string languageCode)
        {
            return new List<HttpCustomHeaderField>
            {
                new HttpCustomHeaderField { HeaderName = "Accept-Language", HeaderValue = languageCode}
            };
        }
    }
}
