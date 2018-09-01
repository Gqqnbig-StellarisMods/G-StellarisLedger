using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace StellarisLedger
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

		    services.AddSingleton<YamlStringLocalizer>();


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
				    var supportedCultures = new HashSet<CultureInfo>
				    {
						CultureInfo.CurrentCulture,
						CultureInfo.CurrentUICulture,
						new CultureInfo("zh"),
					    new CultureInfo("en"),
						new CultureInfo("pt-BR"),
						new CultureInfo("de"),
						new CultureInfo("fr"),
						new CultureInfo("es"),
						new CultureInfo("pl"),
						new CultureInfo("ru")
					};

					// Formatting numbers, dates, etc.
					opts.SupportedCultures = supportedCultures.ToList();
					//// UI strings that we have localized.
					opts.SupportedUICultures = supportedCultures.ToList();
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
