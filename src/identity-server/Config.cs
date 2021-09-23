// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace identity_server
{
    public static class Config
    {
        /// <summary>
        /// Map to scopes that give identity related information
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("resourceapi.fullaccess")
            };

        public static IEnumerable<ApiResource> Apis =>
            new[]
            {
                new ApiResource("resourceapi", "Resource API")
                {
                    Scopes = {"resourceapi.fullaccess"}
                }
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256())},

                    AllowedScopes = {"resourceapi.fullaccess"}
                },

                // MVC client using code flow + pkce
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = true,
                    ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},

                    // The authorization code flow is redirection based, i.e. the authorization code is delivered to the browser via a redirect URI,
                    // therefore a uri is configured for the client to receive the code on. signin-oidc is the default endpoint of the MS openid connect
                    // middleware listens on
                    RedirectUris = {"https://localhost:44371/signin-oidc"},
                    FrontChannelLogoutUri = "https://localhost:44371/signout-oidc",
                    PostLogoutRedirectUris = {"https://localhost:44371/signout-callback-oidc"},

                    AllowOfflineAccess = true,
                    AllowedScopes = {"openid", "profile", "resourceapi.fullaccess"}
                },

                // SPA client using code flow + pkce
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                    },

                    PostLogoutRedirectUris = {"http://localhost:5002/index.html"},
                    AllowedCorsOrigins = {"http://localhost:5002"},

                    AllowedScopes = {"openid", "profile", "resourceapi.fullaccess"}
                }
            };
    }
}