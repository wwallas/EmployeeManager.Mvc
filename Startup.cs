using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager.Mvc.Models;
using EmployeeManager.Mvc.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmployeeManager.Mvc
{
    public class Startup
    {
        private IConfiguration config = null;
        public Startup(IConfiguration config)
        {
            this.config = config;
        }
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<AppDbContext>(
                    options => options.UseSqlServer(this.config.GetConnectionString("AppDb")));

            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(this.config.GetConnectionString("AppDb")));
            services.AddIdentity<AppIdentityUser, AppIdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>();
            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Security/SignIn";
                opt.AccessDeniedPath = "/Security/AccessDenied";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();            

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=EmployeeManager}/{action=List}/{id?}");
                //pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
