using MeterReadingsApi.Data.Repositories;
using MeterReadingsApi.Data.Repositories.Interfaces;
using MeterReadingsApi.Services;
using MeterReadingsApi.Services.Interfaces;

namespace MeterReadingsApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICsvParserService, CsvParserService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IMeterReadingService, MeterReadingService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();

            return services;
        }
    }
}
