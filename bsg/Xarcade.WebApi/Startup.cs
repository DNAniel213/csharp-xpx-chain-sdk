using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Xarcade.Application.Xarcade;
using Xarcade.Application.ProximaX;
using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.ProximaX;
using Xarcade.Infrastructure.Utilities;
using Xarcade.Application.Authentication;
using Xarcade.Application.Authentication.Middleware;

namespace Xarcade.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddTransient<IAccountService, AccountService>();
            //services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IDataAccessProximaX, DataAccessProximaX>();
            services.AddTransient<IBlockchainPortal, ProximaxBlockchainPortal>();
            services.AddControllers();
            services.AddCors(); 
            services.AddScoped<IXarcadeAccountService, XarcadeAccountService>(); 
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IValidator, XarcadeValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
