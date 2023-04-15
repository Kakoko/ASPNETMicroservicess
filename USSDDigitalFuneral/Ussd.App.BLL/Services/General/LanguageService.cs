using AngleDimension.NetCore.Ussd.Localization;
using Ussd.App.BLL;

namespace Ussd.App.BLL.Services.Genenal
{
    public class LanguageService : LanguageServiceBase<LanguageDto>
    {
        public LanguageService(IEnumerable<SupportedLanguage> supportedLanguages, string languageFolder) : base(supportedLanguages, languageFolder)
        {
        }
    }
}