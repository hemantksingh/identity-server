// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Test;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace identity_server4
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddOpenIdConnect("okta", "Okta", options =>
                {
                    options.ClientId = "0oa1i910zrWhY8aEo5d7";
                    options.ClientSecret = "dajXpUruA4Vxn48CIwu8VMG1-azpVZLg82iq96MK";
                    options.Authority = "https://dev-63961725.okta.com";
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.SaveTokens = true;
                    options.Prompt = OpenIdConnectPrompt.SelectAccount;
                });
            // .AddGoogle(options =>
            // {
            //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //
            //     // register your IdentityServer with Google at https://console.developers.google.com
            //     // enable the Google+ API
            //     // set the redirect URI to https://localhost:5000/signin-google
            //     options.ClientId = "copy client ID from Google here";
            //     options.ClientSecret = "copy client secret from Google here";
            // });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/identity", appBuilder =>
            {
                // uncomment if you want to add MVC
                appBuilder.UseStaticFiles();
                appBuilder.UseRouting();

                appBuilder.UseIdentityServer();

                // uncomment, if you want to add MVC
                appBuilder.UseAuthorization();
                appBuilder.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            });
            
        }
    }
}
