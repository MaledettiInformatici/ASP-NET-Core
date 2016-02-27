using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Diagnostics;

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

            // aggiungo il middleware per la gestione della pagina di errore dopo avere impostato su 
            // project.json il riferimento a Microsoft.AspNet.Diagnostics
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseRuntimeInfoPage(); // il path di default è  /runtimeinfo
            }
            else
            {
                // specifico il comportamento che l'applicazione deve eseguire in caso di errore 
                // se la riga seguente viene commentata, l'exception non verrà catturata 
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseWelcomePage("/welcome");


            // genero il response per la creazione di un indice che permetta di linkare o la pagina 
            // di benvenuto alla risorsa /welcome  oppure lancio l'exception per simulare la routine
            app.Run(async (context) =>
            {
                if (context.Request.Query.ContainsKey("throw")) throw new Exception("Exception triggered!");
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<html><body>Hello World!");
                await context.Response.WriteAsync("<ul>");
                await context.Response.WriteAsync("<li><a href=\"/welcome\">Welcome Page</a></li>");
                await context.Response.WriteAsync("<li><a href=\"/?throw=true\">Throw Exception</a></li>");
                await context.Response.WriteAsync("</ul>");
                await context.Response.WriteAsync("</body></html>");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
