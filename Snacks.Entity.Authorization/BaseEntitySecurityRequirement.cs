using Snacks.Entity.Core;

namespace Snacks.Entity.Authorization
{
    public abstract class BaseEntitySecurityRequirement<TModel> : IEntitySecurityRequirement<TModel>
        where TModel : IEntityModel
    {
        
    }
}
