using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebSocketManager;
using BattleShip.GameLogic;
using Microsoft.EntityFrameworkCore;
using BattleShip.Data;

namespace BattleShip
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddDbContext<DataBaseContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));                 
            services.AddWebSocketManager();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseBrowserLink();
            app.UseDeveloperExceptionPage();

            app.UseWebSockets();
            app.MapWebSocketManager("/game", serviceProvider.GetRequiredService<GameHandler>());

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}