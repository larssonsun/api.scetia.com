using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;

namespace IdentityServer
{
    public class DefaultProfileService : IProfileService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefaultProfileService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            System.Console.WriteLine(123);
            
            List<Claim> claims = new List<Claim>();

            var userId = _httpContextAccessor.HttpContext.Request.Form["user_id"].ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                claims.Add(new Claim("user_id", userId));
            }
            
            context.AddRequestedClaims(claims);

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            System.Console.WriteLine(456);
            return Task.CompletedTask;
        }
    }
}