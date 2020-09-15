using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Xarcade.Infrastructure.Abstract;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Xarcade.Application.Authentication.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;
        private readonly IDataAccessProximaX dataAccessAuthentication;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, IDataAccessProximaX dataAccessAuthentication)
        {
            this.next = next;
            this.configuration = configuration;
            this.dataAccessAuthentication = dataAccessAuthentication;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachAccountToContext(context, token);

            await this.next(context);
        }

        private async Task AttachAccountToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(configuration["JwtTokenSettings:Secret"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var jwtAttachId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // attach account to context on successful jwt validation
                var searchKey = new XarcadeUserSearchKey()
                {
                    JwtAttachId = jwtAttachId
                };
                context.Items["Account"] = await Task.Run(() => this.dataAccessAuthentication.LoadXarcadeUser(searchKey));
            }
            catch
            {
                // do nothing if jwt validation fails
                // account is not attached to context so request won't have access to secure routes
            }
        }
    }
}
