using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Snacks.Entity.Core;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Snacks.Entity.Authorization
{
    public class SecuredGlobalCacheService<TEntity> : BaseEntityCacheService<TEntity>
        where TEntity : class
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecuredGlobalCacheService(
            IDistributedCache distributedCache,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService) : base(distributedCache)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
        }

        public override async Task<TEntity> FindAsync(HttpRequest httpRequest)
        {
            TEntity model = await base.FindAsync(httpRequest);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(GetUser(), model, Operations.Read);

            if (authorizationResult.Succeeded)
            {
                return model;
            }

            return default;
        }

        public override async Task<IList<TEntity>> GetAsync(HttpRequest httpRequest)
        {
            IList<TEntity> models = await base.GetAsync(httpRequest);

            foreach (TEntity model in models)
            {
                AuthorizationResult authorizationResult =
                    await _authorizationService.AuthorizeAsync(GetUser(), model, Operations.Read);

                if (!authorizationResult.Succeeded)
                {
                    break;
                }
            }

            return default;
        }

        public override async Task<TValue> GetValueAsync<TValue>(HttpRequest request, TEntity model)
        {
            string cacheKey = GetCacheKey(request);
            Tuple<TEntity, TValue> entityValue = await ReadFromCacheAsync<Tuple<TEntity, TValue>>(cacheKey).ConfigureAwait(false);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(GetUser(), entityValue.Item1, Operations.Read);

            if (authorizationResult.Succeeded)
            {
                return entityValue.Item2;
            }

            return default;
        }

        private ClaimsPrincipal GetUser()
        {
            return _httpContextAccessor.HttpContext.User;
        }
    }
}
