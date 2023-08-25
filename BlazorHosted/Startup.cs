using System;
using Microsoft.AspNetCore.HttpOverrides;
using Lib;

namespace BlazorHosted
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            
            var bindAppSecrets = new AppSecrets();
            Configuration.Bind("AppSecrets", bindAppSecrets);
            services.AddSingleton(bindAppSecrets);
            services.AddHttpClient("api", client =>
            {
                client.BaseAddress = new Uri(bindAppSecrets.BaseAddress); // Replace with your server URL
            });
            services.AddControllersWithViews(options =>
            {
                /*var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));*/
            });
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddControllersWithViews(options =>
            {
                /*var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));*/
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // must happen before HSTS
                app.UseForwardedHeaders();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // caused the reply_url to start with https
            /*app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });*/
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
