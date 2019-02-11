using Marco.Http.Client.Abstractions;
using Marco.Http.Client.AspNetCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
   public static class MarcoAspNetCoreHttpClientSeviceCollectionExtensions
    {
        public static IServiceCollection AddAspNetCoreHttpClientFactory(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped<IMarcoHttpClientFactory, MarcoHttpClientFactory>();

            return services;
        }
    }
}