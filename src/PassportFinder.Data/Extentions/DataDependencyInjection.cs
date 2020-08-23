using Microsoft.Extensions.DependencyInjection;
using PassportFinder.Data.Abstractions;
using PassportFinder.Data.HtmlFinders;
using System.Net.Http;

namespace PassportFinder.Data.Extentions
{
    public static class DataDependencyInjection
    {
        public static IServiceCollection AddDataServicesDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient<DPFApoitmentAlertsFinder>();
            services.AddTransient<DPFCityFinder>();
            services.AddTransient<DPFOfficeFinder>();
            services.AddTransient<FormActionFinder>();
            services.AddTransient<HttpClient>();
            services.AddTransient<IDPFGateway, DPFGateway>();
            return services;
        }
    }
}
