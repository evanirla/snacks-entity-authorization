using Microsoft.AspNetCore.Authorization;
using Snacks.Entity.Core.Entity;

namespace Snacks.Entity.Authorization
{
    public interface IEntitySecurityRequirement<TModel> : IAuthorizationRequirement
        where TModel : IEntityModel
    {
    }
}
