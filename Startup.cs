using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BethanysPieShop.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BethanysPieShop
{
    public class Startup
    {
        private IHostingEnvironment _env;

        private IConfigurationRoot _conigurationRoot;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            _conigurationRoot = builder.Build();

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton(_conigurationRoot);

            services.AddDbContext<AppDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IPieRepository, PieRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //allows to work with context

            //add scoped=
            //creating object associated with request, but are different for each requests
            //so when i browse through the site, ill get an instance of the shopping cart, if another does it, then they get one also
            services.AddScoped<ShoppingCart>(sp => ShoppingCart.GetCart(sp)); 

            services.AddMvc();

            services.AddMemoryCache();
            services.AddSession();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //adding basic middleware components
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages(); //like 404 or 500
            app.UseStaticFiles();
            app.UseSession();
            app.UseIdentity();

            //app.UseMvcWithDefaultRoute();
            app.UseMvc(routes =>
            {
            routes.MapRoute(
                name: "categoryfilter",
                template: "Pie/{action}/{category?}",
                defaults: new { Controller = "Pie", action = "List" }
                    );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                    );
            });

            DbInitializer.Seed(app); //invoke seed everytime app is loaded
        }
    }
}
