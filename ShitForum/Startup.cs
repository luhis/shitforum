using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using Hashers;
using MediaToolkit.Util;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using ReCaptchaCore;
using ThumbNailer;
using ShitForum.Analytics;

namespace ShitForum
{
    public class Startup
    {
        public Startup(ILogger<Startup> logger, IConfiguration conf)
        {
            logger.LogInformation($"Starting up ShitForum {DateTime.UtcNow}");
            this.conf = conf;
        }

        private static readonly IEnumerable<Action<IServiceCollection>> DIModules = new Action<IServiceCollection>[]
        {
            Persistence.DIModule.Add,
            Services.DIModule.Add,
            ReCaptchaCore.DIModule.Add,
            ThumbNailer.DIModule.Add,
            ShitForum.DIModule.Add,
            Cookies.DIModule.Add,
            ExtremeIpLookup.DIModule.Add
        };

        private readonly IConfiguration conf;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            DIModules.ForEach(b => b(services));
            services.Configure<RecaptchaSettings>(conf.GetSection("Recaptcha"));
            services.Configure<IpHasherSettings>(conf.GetSection("IpHash"));
            services.Configure<ThumbNailerSettings>(conf);
            services.Configure<TripCodeHasherSettings>(conf);
            services.Configure<AdminSettingsRaw>(conf);

            services.Configure<RouteOptions>(options => {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = false;
            });
            services.AddMvc().AddRazorPagesOptions(o => o.Conventions.AddPageRoute("/thread", "board/{boardKey}/{threadId}"));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => options.AccessDeniedPath = "/Login");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger, IThumbNailer thumbNailer, ForumContext forumContext)
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

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            var google = "https://www.google.com";
            app.UseCsp(a => a.
                DefaultSources(b => b.Self()).
                ImageSources(c => c.Self().CustomSources("data:")).
                StyleSources(c => c.Self().UnsafeInline()).
                ScriptSources(s => s.Self().CustomSources(google, "https://www.gstatic.com")).
                FrameSources(c => c.Self().CustomSources(google)));
            app.UseXContentSecurityPolicy();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse =
                    r =>
                    {
                        string path = r.File.PhysicalPath;
                        var cacheExtensions = new[] { ".css", ".js", ".gif", ".jpg", ".png", ".svg" };
                        if (cacheExtensions.Any(path.EndsWith))
                        {
                            var maxAge = TimeSpan.FromDays(7);
                            r.Context.Response.Headers.Add("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
                        }
                    }
            });
            app.UseXfo(options => options.Deny());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXContentTypeOptions();
            app.UseXDownloadOptions();
            app.UseRedirectValidation();
            app.UseAnalyticsMiddleware();
            app.UseExpectCt();

            app.UseMvc();
            
            forumContext.SeedData().Wait();
            
            if (!thumbNailer.IsSettingValid())
            {
                logger.LogInformation("cannot locate FFMPEG executable.");
            }
        }
    }
}
