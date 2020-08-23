using Microsoft.Extensions.DependencyInjection;
using PassportFinder.Data;
using PassportFinder.Data.Abstractions;
using PassportFinder.Data.HtmlFinders;
using PassportFinder.Service;
using PassportFinder.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PassportFinder.Service.Extentions
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServicesDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient<IEmailNotifier, EmailNotifier>();
            services.AddTransient<IAppointmentsAvailbilityReportGenerator, AppointmentsAvailbilityReportGenerator>();
            services.AddTransient<IAppointmentsAvailbilityReportSender, AppointmentsAvailbilityReportSender>();            
            return services;
        }
    }
}
