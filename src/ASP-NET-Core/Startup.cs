using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Diagnostics;

using Microsoft.Extensions.Configuration;               // libreria per l'accesso alle configurazioni 

namespace ASP_NET_Core
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseIISPlatformHandler();

            // aggiungo il middleware per la gestione dell'errore (project.json --> Microsoft.AspNet.Diagnostics)
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseRuntimeInfoPage();           // il path di default è  /runtimeinfo
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");  // risorsa da cercare in caso di exception 
            }
            app.UseWelcomePage("/welcome");         // risorsa per il welcome 

            // carico le configurazioni definite su project.json 
            LoadSettings();

            // genero response per invocare la welcome o l'errore 
            app.Run(async (context) =>
            {
                if (context.Request.Query.ContainsKey("throw")) throw new Exception("Exception triggered!");
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<html><body>Hello World!");
                await context.Response.WriteAsync(string.Format("<p>impostazioni: </br> -->key1 = {0}</br> -->key2 = {1}</p>", MySettings[0], MySettings[1]));
                await context.Response.WriteAsync(string.Format("<p>impostazioni: </br> -->key3 = {0}</br> -->key4 = {1}</p>", MySettings[2], MySettings[3]));
                await context.Response.WriteAsync("<ul>");
                await context.Response.WriteAsync("<li><a href=\"/welcome\">Welcome Page</a></li>");
                await context.Response.WriteAsync("<li><a href=\"/?throw=true\">Throw Exception</a></li>");
                await context.Response.WriteAsync("</ul>");
                await context.Response.WriteAsync("</body></html>");
            });
        }

        public static string[] MySettings = new string[4];


        /// <summary>
        /// Load Settings from resource json files
        /// </summary>
        public static void LoadSettings()
        {
            // impostazione di una configurazione 
            // var builder = new ConfigurationBuilder().AddJsonFile("project.json");

            // impostazione di una configurazione su più risorse 
            var builder = new ConfigurationBuilder()
                    .AddJsonFile("project.json")
                    .AddJsonFile("appsettings.json", optional: true);

            var config = builder.Build();

            // estrazione della configurazione command:web
            //string commands = config["commands:web"];

            MySettings[0] = config["mysettings:key1"];
            MySettings[1] = config["mysettings:key2"];
            MySettings[2] = config["mysettings:key3"];
            MySettings[3] = config["mysettings:key4"];
        }


        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
