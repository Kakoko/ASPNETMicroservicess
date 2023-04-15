using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using StackExchange.Redis.Extensions.System.Text.Json;
using StackExchange.Redis.Extensions.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ussd.App.Worker;
using MassTransit;
using Microsoft.Extensions.Configuration;



await CreateHostBuilder(args).Build().RunAsync();


Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .UseSerilog((context, config) =>
              {
                  config.WriteTo
                  .Console()
                  .ReadFrom.Configuration(context.Configuration);

              })
              .ConfigureServices((hostContext, services) =>
              {
                  var configuration = hostContext.Configuration;

                  services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>((options) =>
                  {
                      return new[] { configuration.GetSection("Redis").Get<RedisConfiguration>() };
                  });
                  services.AddDomainServices(configuration, hostContext.HostingEnvironment);
                  services.Configure<RabbitMqTransportOptions>(hostContext.Configuration.GetSection("RabbitMq"));
                  services.AddMassTransit(x =>
                  {
                      x.SetKebabCaseEndpointNameFormatter();
                      x.AddConsumers(Assembly.GetEntryAssembly());
                      x.UsingRabbitMq((context, config) =>
                      {
                          config.ConfigureEndpoints(context);
                      });
                  });
                  services.AddOptions<MassTransitHostOptions>().Configure(options =>
                  {
                      options.WaitUntilStarted = true;
                  });
              });