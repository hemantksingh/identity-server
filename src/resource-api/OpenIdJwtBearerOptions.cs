using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace resource_api
{
    public class OpenIdJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        public void Configure(JwtBearerOptions options)
        {
            Configure(Options.DefaultName, options);
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            options.Authority = "https://localhost:5000/identity";
            if (string.IsNullOrEmpty(options.Audience))
            {
                options.Audience = "";
            }
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                //IssuerSigningKey = _securityKey,
                //RequireExpirationTime = true,
                //RequireSignedTokens = true
            };
        }
    }
}
