using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Snacks.Entity.Authorization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Snacks.Entity.Authorization.Extensions
{
    public static class ServiceCollectionExtensions
    {
        static IEnumerable<Type> EntitySecurityHandlers =>
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.FullName.Contains("Snacks.Entity.Authorization"))
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsAbstract && !x.IsInterface)
                .Where(x => x.IsAssignableToGenericType(typeof(BaseEntitySecurityHandler<,>)));

        static IEnumerable<Type> EntitySecurityRequirements =>
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.FullName.Contains("Snacks.Entity.Authorization"))
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsAbstract && !x.IsInterface)
                .Where(x => x.IsAssignableToGenericType(typeof(IEntitySecurityRequirement<>)));

        public static IServiceCollection AddEntityAuthorization(this IServiceCollection services, Action<AuthorizationOptions> configure = null)
        {
            services.AddAuthorizationCore(options =>
            {
                foreach (Type requirement in EntitySecurityRequirements)
                {
                    PolicyAttribute policyAttribute = requirement.GetCustomAttribute<PolicyAttribute>();

                    string policyName = policyAttribute?.Name ?? (GetModelType(requirement).Name + "Security");

                    options.AddPolicy(policyName, policy =>
                    {
                        policy.Requirements.Add((IAuthorizationRequirement)Activator.CreateInstance(requirement));
                    });
                }

                if (configure != null)
                {
                    configure.Invoke(options);
                }
            });

            foreach (Type handler in EntitySecurityHandlers)
            {
                services.AddSingleton(typeof(IAuthorizationHandler), handler);
            }

            return services;
        }

        private static Type GetModelType(Type requirementType)
        {
            Type modelType = requirementType.BaseType.GetGenericArguments().FirstOrDefault();

            if (modelType == null)
            {
                modelType = requirementType.GetInterface("IEntitySecurityRequirement`1").GetGenericArguments().FirstOrDefault();
            }

            return modelType;
        }
    }
}
