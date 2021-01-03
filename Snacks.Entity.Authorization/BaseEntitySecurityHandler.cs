using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Snacks.Entity.Authorization.Attributes;
using Snacks.Entity.Core;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Snacks.Entity.Authorization
{
    public abstract class BaseEntitySecurityHandler<TModel, TRequirement> : AuthorizationHandler<TRequirement>
        where TModel : IEntityModel
        where TRequirement : IEntitySecurityRequirement<TModel>
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public BaseEntitySecurityHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext.Request.Method == HttpMethods.Get && typeof(TModel).IsDefined(typeof(RestrictReadAttribute)))
            {
                RestrictReadAttribute restrictReadAttribute = typeof(TModel).GetCustomAttribute<RestrictReadAttribute>();

                if (!restrictReadAttribute.Roles.Any(x => context.User.IsInRole(x)))
                {
                    context.Fail();
                }
            }
            else if (httpContext.Request.Method == HttpMethods.Post && typeof(TModel).IsDefined(typeof(RestrictCreateAttribute)))
            {
                RestrictCreateAttribute restrictCreateAttribute = typeof(TModel).GetCustomAttribute<RestrictCreateAttribute>();

                if (!restrictCreateAttribute.Roles.Any(x => context.User.IsInRole(x)))
                {
                    context.Fail();
                }
            }
            else if (httpContext.Request.Method == HttpMethods.Patch && typeof(TModel).IsDefined(typeof(RestrictUpdateAttribute)))
            {
                RestrictUpdateAttribute restrictUpdateAttribute = typeof(TModel).GetCustomAttribute<RestrictUpdateAttribute>();

                if (!restrictUpdateAttribute.Roles.Any(x => context.User.IsInRole(x)))
                {
                    context.Fail();
                }
            }
            else if (httpContext.Request.Method == HttpMethods.Delete && typeof(TModel).IsDefined(typeof(RestrictDeleteAttribute)))
            {
                RestrictDeleteAttribute restrictDeleteAttribute = typeof(TModel).GetCustomAttribute<RestrictDeleteAttribute>();

                if (!restrictDeleteAttribute.Roles.Any(x => context.User.IsInRole(x)))
                {
                    context.Fail();
                }
            }

            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
