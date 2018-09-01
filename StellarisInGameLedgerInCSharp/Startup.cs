using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace StellarisInGameLedgerInCSharp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

		//被Program.BuildWebHost隐式调用。Use this method to add services to the container.
	    // ReSharper disable once UnusedMember.Global
	    public void ConfigureServices(IServiceCollection services)
	    {
		    services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

			


			services.AddMvc()
					.AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix,
										 opt => { opt.ResourcesPath = "Resources"; })
					.AddWebApiConventions()
					.AddJsonOptions(options =>
		            {
#if DEBUG
						options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
#else
			            options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
#endif
					});

			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

		    services.Configure<RequestLocalizationOptions>(
			    opts =>
			    {
				    var supportedCultures = new List<CultureInfo>
				    {
					    new CultureInfo("zh-CN"),
					    new CultureInfo("en-US"),
					    new CultureInfo("zh"),
					    new CultureInfo("en")
				    };

				    opts.DefaultRequestCulture = new RequestCulture("zh-CN");
				    // Formatting numbers, dates, etc.
				    opts.SupportedCultures = supportedCultures;
				    // UI strings that we have localized.
				    opts.SupportedUICultures = supportedCultures;
			    });
		}

	    //被Program.BuildWebHost隐式调用。Use this method to configure the HTTP request pipeline.
	    // ReSharper disable once UnusedMember.Global
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

	        var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
	        app.UseRequestLocalization(options.Value);

			app.UseMvc();
        }
    }
}
