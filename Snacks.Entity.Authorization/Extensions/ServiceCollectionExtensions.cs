using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snacks.Entity.Authorization.Extensions
{
    public static class ServiceCollectionExtensions
    {
        static IEnumerable<Type> EntitySecurityHandlers =>
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.FullName.Contains("Snacks.Entity.Authorization"))
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(BaseEntitySecurityHandler<,>).IsAssignableFrom(x))
                .Where(x => !x.IsAbstract && !x.IsInterface);

        static IEnumerable<Type> EntitySecurityRequirements =>
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.FullName.Contains("Snacks.Entity.Authorization"))
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IEntitySecurityRequirement<>).IsAssignableFrom(x))
                .Where(x => !x.IsAbstract && !x.IsInterface);

        public static IServiceCollection AddEntitySecurity(this IServiceCollection services)
        {
            foreach (Type handler in EntitySecurityHandlers)
            {
                services.AddSingleton(typeof(IAuthorizationHandler), Activator.CreateInstance(handler));
            }

            return services;
        }
    }
}
