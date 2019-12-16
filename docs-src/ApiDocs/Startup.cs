using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace ApiDocs
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var options = new FileServerOptions()
            {
                EnableDefaultFiles = true
            };
            options.StaticFileOptions.DefaultContentType = @"text/html";
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.StaticFileOptions.FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, @"..", @"..", @"docs"));
            options.DefaultFilesOptions.DefaultFileNames.Add(@"index.html");
            app.UseFileServer(options);
        }
    }
}