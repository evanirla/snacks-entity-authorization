using Snacks.Entity.Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snacks.Entity.Authorization
{
    public abstract class BaseEntitySecurityRequirement<TModel> : IEntitySecurityRequirement<TModel>
        where TModel : IEntityModel
    {
        
    }
}
