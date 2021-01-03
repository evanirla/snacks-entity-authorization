using Microsoft.AspNetCore.Authorization;
using Snacks.Entity.Core;

namespace Snacks.Entity.Authorization
{
    public interface IEntitySecurityRequirement<TModel> : IAuthorizationRequirement
        where TModel : IEntityModel
    {
    }
}
