using Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;
using Persistence.Repositories;
using ShitForum.Hasher;
using ShitForum.ImageValidation;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using ShitForum.BannedImageLogger;
using ShitForum.Cookies;
using ShitForum.GetIp;

namespace ShitForum
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            this.Configuration = configuration;
            logger.LogInformation($"Starting up ShitForum {DateTime.UtcNow}");
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Persistence.DIModule.Add(services);
            Services.DIModule.Add(services);
            ReCaptchaCore.DIModule.Add(services);
            services.AddSingleton<IFileRepository, FileRepository>();
            services.AddSingleton<IValidateImage, ValidateImage>();
            services.AddSingleton<ICookieStorage, CookieStorage>();
            services.AddSingleton<IBannedImageLogger, BannedImageLogger.BannedImageLogger>();
            services.AddSingleton<IGetIp, GetIp.GetIp>();
            services.AddSingleton<IAdminChecker, AdminChecker>();
            services.AddSingleton<ForumContext>();
            services.AddSingleton<IpHasherFactory>();
            services.AddSingleton<TripCodeHasher>();
            services.AddSingleton<AdminSettings>();

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
                        context.Response.Headers["Location"] = context.RedirectUri;
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
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts(a => a.MaxAge(365));
            }

            app.UseCsp(a => a.
                DefaultSources(b => b.Self()).
                ImageSources(c => c.Self()).
                StyleSources(c => c.Self().UnsafeInline()).
                ScriptSources(s => s.Self().CustomSources("https://www.google.com", "https://www.gstatic.com")).
                FrameSources(c => c.Self().CustomSources("https://www.google.com")));
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseStaticFiles();
            app.UseRedirectValidation();
            app.UseXfo(options => options.Deny());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXContentTypeOptions();
            app.UseXDownloadOptions();

            app.UseMvc();

            var fac = app.ApplicationServices.GetService<ForumContext>();
            fac.SeedData().Wait();
        }
    }
}
