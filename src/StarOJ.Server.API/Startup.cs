using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace StarOJ.Server.API
{
    public class Startup
    {
        public class AppConfig
        {
            public string FileStoreRoot { get; set; }
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddSingleton<StarOJ.Core.IWorkspace>(new Data.Provider.FileSystem.Workspace(Path.GetFullPath(Directory.GetCurrentDirectory())));
            services.AddDbContext<Data.Provider.SqlServer.Models.OJContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var appconfig = Configuration.GetSection("AppConfig").Get<AppConfig>();

            services.AddScoped((provider) =>
            {
                return new StarOJ.Data.Provider.SqlServer.WorkspaceStartup
                {
                    FileStoreRoot = appconfig.FileStoreRoot
                };
            });
            services.AddScoped<StarOJ.Core.IWorkspace, StarOJ.Data.Provider.SqlServer.Workspace>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
