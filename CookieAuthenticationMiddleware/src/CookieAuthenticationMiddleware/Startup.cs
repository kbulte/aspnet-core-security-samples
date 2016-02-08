using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Http;

namespace CookieAuthenticationMiddleware
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseCookieAuthentication(options => {
                //In the old we had forms and windows since katana we can have multiple
                options.AuthenticationScheme = "Cookies";
                options.LoginPath = new PathString("/Account/Login");
                //When you hit a secured page which you are not allowed go to the given path instead of going back to the login view
                options.AccessDeniedPath = new PathString("/Account/Forbidden");

                //AutomaticAuthenticate: determines whether the authentication scheme will call Authenticate and merge any claims principals produced into HttpContext.User
                //See https://github.com/aspnet/Announcements/issues/83
                //Both are set to false by default
                options.AutomaticAuthenticate = true; //Way in 
                //Determines whether the authentication scheme will handle any naked Challenges, i.e. [Authorize], or any explicit challenge calls where the scheme wasn't specified
                options.AutomaticChallenge = true; //Way out
            });

            //Make even static files protected by the cookie
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
