using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityServer
{
    public static class Config
    {
        // public static IEnumerable<IdentityResource> GetIdentityResources()
        // {
        //     var scetiaUserProfile = new IdentityResource(
        //     "scetia.user.profile",
        //     "scetia.user.profile",
        //     new[]
        //     {
        //         "user_id"
        //     }
        // );

        //     return new List<IdentityResource>
        //     {
        //         new IdentityResources.OpenId(),
        //         new IdentityResources.Profile(),
        //         scetiaUserProfile
        //     };
        // }

        public static IEnumerable<ApiScope> GetApiScopes(IConfigurationSection section)
        {
            var apiScpoes = new List<ApiScope>();
            List<ScopeConfig> configs = new List<ScopeConfig>();
            section.Bind("Scopes", configs);
            apiScpoes.AddRange(configs.Select(x => new ApiScope(x.Name, x.DisplayName)));
            return apiScpoes;
        }

        public static IEnumerable<ApiResource> GetApiResources(IConfigurationSection section)
        {
            var apiResources = new List<ApiResource>();
            List<ApiConfig> configs = new List<ApiConfig>();
            section.Bind("ApiResources", configs);
            apiResources.AddRange(configs.Select(conf =>
            {
                var ar = new ApiResource(conf.Name, conf.DisplayName, conf.UserClaims);
                conf.ScopesNames?.Any(y =>
                {
                    ar.Scopes.Add(y);
                    return false;
                });
                return ar;
            }));

            return apiResources;
        }

        public static IEnumerable<Client> GetClients(IConfigurationSection section)
        {
            var clients = new List<Client>();
            List<ClientConfig> configs = new List<ClientConfig>();
            section.Bind("Clients", configs);
            foreach (var config in configs)
            {
                Client client = new Client();
                client.ClientId = config.ClientId;
                List<Secret> clientSecrets = new List<Secret>();
                foreach (var secret in config.ClientSecrets)
                {
                    clientSecrets.Add(new Secret(secret.Sha256()));
                }
                client.ClientSecrets = clientSecrets.ToArray();
                GrantTypes grantTypes = new GrantTypes();
                var allowedGrantTypes = grantTypes.GetType().GetProperty(config.AllowedGrantTypes);
                client.AllowedGrantTypes = allowedGrantTypes == null ?
                    GrantTypes.ClientCredentials : (ICollection<string>)allowedGrantTypes.GetValue(grantTypes, null);

                client.AllowedScopes.Add(IdentityServerConstants.StandardScopes.OpenId);
                client.AllowedScopes.Add(IdentityServerConstants.StandardScopes.Profile);
                config.AllowedScopes?.Any(a => { client.AllowedScopes.Add(a); return false; });

                clients.Add(client);
            }
            return clients;
        }
    }

    public class ScopeConfig
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class ApiConfig
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<string> ScopesNames { get; set; }
        public IEnumerable<string> UserClaims { get; set; }
    }

    public class ClientConfig
    {
        public string ClientId { get; set; }
        public List<string> ClientSecrets { get; set; }
        public string AllowedGrantTypes { get; set; }
        public List<string> AllowedScopes { get; set; }
    }
}