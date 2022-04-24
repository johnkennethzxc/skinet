using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Errors;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // This is added in ApplicationServicesExtensions
            // services.AddScoped<IProductRepository, ProductRepository>();

            // services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));

            services.AddAutoMapper(typeof(MappingProfiles));

            services.AddControllers();

            // This is added in ApplicationServicesExtensions
            // services.Configure<ApiBehaviorOptions>(options => {
            //     options.InvalidModelStateResponseFactory = actionContext =>
            //     {
            //         var errors = actionContext.ModelState
            //             .Where(e => e.Value.Errors.Count > 0)
            //             .SelectMany(x => x.Value.Errors)
            //             .Select(x => x.ErrorMessage)
            //             .ToArray();

            //         var errorResponse = new ApiValidationErrorResponse 
            //         {
            //             Errors = errors
            //         };

            //         return new BadRequestObjectResult(errorResponse);
            //     };
            // });
            
            services.AddDbContext<StoreContext>(x => 
                x.UseSqlServer(_config.GetConnectionString("DefaultConnection")));

            services.AddDbContext<AppIdentityDbContext>(x => {
                x.UseSqlServer(_config.GetConnectionString("IdentityConnection"));
            });

            services.AddSingleton<IConnectionMultiplexer>(c => {
                var configuration = ConfigurationOptions.Parse(_config
                    .GetConnectionString("Redis"), true);

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddApplicationServices();

            services.AddIdentityServices(_config);

            services.AddSwaggerDocumentation();

            // services.AddCors(opt => 
            // {
            //     opt.AddPolicy("CorsPolicy", policy => {
            //         policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
            //     });
            // });

            services.AddCors();
            
            // This is added in SwaggerServiceExtensions    
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            // This is added in SwaggerServiceExtensions 
            // app.UseSwagger();
            // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

            // if (env.IsDevelopment())
            // {
            //     // This is already rewritten when using Exception Middleware.
            //     // app.UseDeveloperExceptionPage();

            //     // Also added in production
            //     // app.UseSwagger();
            //     // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            // }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            // app.UseCors("CorsPolicy");

            app.UseCors(x => x.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("https://localhost:4200"));

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwaggerDocumentation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
