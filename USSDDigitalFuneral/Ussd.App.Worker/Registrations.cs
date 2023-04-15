using AngleDimension.NetCore.Ussd.Abstractions;
using AngleDimension.NetCore.Ussd.Localization;
using AngleDimension.NetCore.Ussd.SessionManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ussd.App.BLL;
using Ussd.App.BLL.Services.Genenal;
using Ussd.App.BLL.Sessions;

namespace Ussd.App.Worker
{
    public static class Registrations
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services,
           IConfiguration configuration, IHostEnvironment environment)
        {


       
            //for supported languages
            var supportedLanguages = GetSupportedLanguages(configuration, environment);
            services.AddScoped<ILanguageService<LanguageDto>>(x => new LanguageService(supportedLanguages, configuration["LanguageFolder"]));


            services.AddScoped<ISessionManager<LanguageDto>, UssdSessionManager>();

            //build service provider for all added services
            var serviceProvider = services.BuildServiceProvider();
       
            services.AddScoped<IUssdRequestService>(x => new UssdRequestService(serviceProvider));

            return services;
        }

        private static IEnumerable<SupportedLanguage> GetSupportedLanguages(IConfiguration configuration,
            IHostEnvironment environment)
        {
            var supportedLanguagesPath = Path.Combine(environment.ContentRootPath,
                configuration["LanguageFolder"], "supported-languages.json");
            string json = File.ReadAllText(supportedLanguagesPath);
            var supportedLlanguages = JsonSerializer.Deserialize<IEnumerable<SupportedLanguage>>(json);

            return supportedLlanguages;
        }

    }
}
