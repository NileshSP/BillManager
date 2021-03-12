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
using System.Linq;

namespace BillManagerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public Func<IWebHostEnvironment, string> getDBName = (IWebHostEnvironment env) => $"BillManagerApiMainDB-{env.EnvironmentName}";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Get dbcontext instance if exists.... 
            var descriptor = services.SingleOrDefault(
                                        d => d.ServiceType ==
                                            typeof(DbContextOptions<BillManagerDBContext>));

            //services.Remove(descriptor);

            if (descriptor == null)
            {
                services.AddDbContext<BillManagerDBContext>(options =>
                                                // For In memory
                                                options.UseInMemoryDatabase(getDBName(Environment))
                                            );
            }
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
                        Title = $"Bill Manager API({Environment.EnvironmentName})",
                        Version = "v1",
                        Description = $"ASP.NET 5 Web API exposing Bill & Friend entities with relative bill share interactions between them <br/><br/>DB name - {getDBName(Environment)}",
                        Contact = new OpenApiContact
                        {
                            Name = "Nilesh Patel",
                            Email = "emailnileshsp@gmail.com",
                            Url = new Uri("https://nileshsp.github.io/personalprofile")
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT",
                            Url = new Uri("https://choosealicense.com/licenses/mit/")
                        }
                    }); ;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IDBContext dBContext)
        {
            app.UseDeveloperExceptionPage();

            if (!Environment.EnvironmentName.ToLower().Contains("development"))
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Get environment prefix
            var envTitle = Environment.EnvironmentName.ToLower().Contains("dev")
                                ? "DEV"
                                : (Environment.EnvironmentName.ToLower().Contains("prod") 
                                    ? "PROD" 
                                    : "UAT");

            //app.UseCustomCorsMiddleware();

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = $"{envTitle} - Bill Manager";
                c.RoutePrefix = "";
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Version 1");
            });
            app.UseMvc();

            dBContext.SeedSampleData().Wait();
        }
    }
}