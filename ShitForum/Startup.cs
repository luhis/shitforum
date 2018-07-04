using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using ThumbNailer;
using ShitForum.Analytics;

namespace ShitForum
{
    public class Startup
    {
        public Startup(ILogger<Startup> logger)
        {
            logger.LogInformation($"Starting up ShitForum {DateTime.UtcNow}");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Persistence.DIModule.Add(services);
            Services.DIModule.Add(services);
            ReCaptchaCore.DIModule.Add(services);
            ThumbNailer.DIModule.Add(services);
            ShitForum.DIModule.Add(services);

            services.Configure<RouteOptions>(options => {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = false;
            });
            services.AddMvc();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Login";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
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
            app.UseAnalyticsMiddleware();

            app.UseMvc();

            var fac = app.ApplicationServices.GetService<ForumContext>();
            fac.SeedData().Wait();

            var thumbNailer = app.ApplicationServices.GetService<IThumbNailer>();
            if (!thumbNailer.IsSettingValid())
            {
                logger.LogInformation("cannot locate FFMPEG executable.");
            }
        }
    }
}
