using BillManagerApi.Repositories;
using BillManagerApi.Repositories.Interfaces;
using BillManagerApi.Services;
using BillManagerApi.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace BillManagerApi
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
            services.AddDbContext<BillManagerDBContext>(options =>
                                            // For In memory
                                            options.UseInMemoryDatabase("BillManagerApiMainDB")
                                        );
            services.AddTransient<IDBContext, BillManagerDBContext>();
            services.AddTransient<IBillService, BillService>();
            services.AddTransient<IFriendService, FriendService>();
            services.AddLogging();
            services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true)
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

            // For API documentation UI
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", 
                    new OpenApiInfo 
                    { 
                        Title = "Bill Manager API", 
                        Version = "v1",
                        Description = "ASP.NET 5 Web API exposing Bill & Friend entities with relative bill share interactions between them",
                        Contact = new OpenApiContact
                        {
                            Name = "Nilesh Patel",
                            Email = "emailnileshsp@gmail.com",
                            Url = new Uri("https://nileshsp.github.io/personalprofile")
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT",
                        }
                    });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDBContext dBContext)
        {
            app.UseDeveloperExceptionPage();
            dBContext.SeedSampleData().Wait();

            if (!env.EnvironmentName.ToLower().Contains("development"))
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseCustomCorsMiddleware();

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Bill Manager";
                c.RoutePrefix = "";
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Version 1");
            });
            app.UseMvc();
        }
    }
}
