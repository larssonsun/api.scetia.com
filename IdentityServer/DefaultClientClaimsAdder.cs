using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;

namespace IdentityServer
{
    public class DefaultClientClaimsAdder : ICustomTokenRequestValidator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefaultClientClaimsAdder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            if (context.Result.ValidatedRequest.GrantType != "client_credentials")
            {
                return Task.CompletedTask;
            }

            // Get userClaims names required by APIRecourse
            var clientAllowedScopes = context.Result.ValidatedRequest.Client.AllowedScopes;
            var userClaimTypes = context.Result.ValidatedRequest.ValidatedResources.Resources.ApiResources
                .Where(x => x.Scopes.Any(y => clientAllowedScopes.Contains(y)))
                .SelectMany(x => x.UserClaims);

            // Add the userClaimTypes in the cliams submitted by the user to the clientCliams
            bool CompletedClaims = true;
            context.Result.ValidatedRequest.Client.AlwaysSendClientClaims = true;
            var clientClaims = context.Result.ValidatedRequest.ClientClaims;
            foreach (var userClaimType in userClaimTypes)
            {
                var newClaimsVal = _httpContextAccessor.HttpContext.Request.Form[userClaimType].ToString();
                if (!string.IsNullOrEmpty(newClaimsVal))
                {
                    clientClaims.Add(new Claim(userClaimType, newClaimsVal));
                }
                else
                    CompletedClaims = false;
            }

            // Add claim "CompletedClaims" when userClaims was completed (for ocelot RouteClaimsRequirement)
            if (CompletedClaims)
            {
                clientClaims.Add(new Claim(nameof(CompletedClaims), "true"));
            }

            return Task.CompletedTask;
        }
    }
}