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
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDBContext dBContext)
        {
            if (env.EnvironmentName.ToLower().Contains("development"))
            {
                app.UseDeveloperExceptionPage();
                dBContext.SeedSampleData().Wait();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseCustomCorsMiddleware();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
