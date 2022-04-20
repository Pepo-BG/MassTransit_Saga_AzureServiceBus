using Automatonymous;
using Azure.Identity;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core.Configurators;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit_Saga_AzureSB.Data;
using Microsoft.EntityFrameworkCore;

namespace MassTransit_Saga_AzureSB
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterStateMachine<TStateMachine, T, TDbContext, TDbContextImplementation>(this IServiceCollection services, string connectionString, ConfigurationManager config) where TStateMachine : class, SagaStateMachine<T> where T : class, SagaStateMachineInstance where TDbContext : DbContext where TDbContextImplementation : DbContext, TDbContext
        {
            services.AddGenericRequestClient();

            services.AddMassTransit(cfg =>
            {
                cfg.AddSagaStateMachine<TStateMachine, T>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic; //requires RowVersion!

                        r.AddDbContext<TDbContext, TDbContextImplementation>((provider, builder) =>
                            {
                                builder.UseSqlServer(connectionString, m =>
                                {
                                    m.MigrationsAssembly(typeof(TDbContextImplementation).Assembly.GetName().Name);
                                    m.MigrationsHistoryTable($"__{typeof(TDbContextImplementation).Name}");
                                });
                            });
                    });

                cfg.UsingAzureServiceBus((context, x) =>
                {
                    var settings = new HostSettings
                    {
                        ServiceUri = new Uri(config.GetValue<string>("ServiceBusHost")),
                        TokenCredential = new DefaultAzureCredential() // From Azure.Identity.dll
                    };
                    x.Host(settings);
                    x.ConfigureEndpoints(context);
                });

            });
            services.AddMassTransitHostedService(true);

            return services;
        }

        public static void ApplyMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<StateDbContext>();
                dataContext.Database.Migrate();
            }
        }
    }
}
