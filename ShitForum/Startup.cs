using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ShitForum
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            logger.LogInformation($"Starting up ShitForum {DateTime.UtcNow}");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Persistence.DIModule.Add(services);
            Services.DIModule.Add(services);
            ReCaptchaCore.DIModule.Add(services);
            ShitForum.DIModule.Add(services);

            services.Configure<RouteOptions>(options => {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = false;
            });
            services.AddMvc();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.RedirectUri = "~/Login";
                        context.Response.Headers["Location"] = "~/Login";
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                /////app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts(a => a.MaxAge(365));
            }

            var google = "https://www.google.com";
            app.UseCsp(a => a.
                DefaultSources(b => b.Self()).
                ImageSources(c => c.Self().CustomSources("data:")).
                StyleSources(c => c.Self().UnsafeInline()).
                ScriptSources(s => s.Self().CustomSources(google, "https://www.gstatic.com")).
                FrameSources(c => c.Self().CustomSources(google)));
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseStaticFiles();
            app.UseRedirectValidation();
            app.UseXfo(options => options.Deny());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXContentTypeOptions();
            app.UseXDownloadOptions();
            app.UseRedirectValidation();

            app.UseMvc();

            var fac = app.ApplicationServices.GetService<ForumContext>();
            fac.SeedData().Wait();
        }
    }
}
