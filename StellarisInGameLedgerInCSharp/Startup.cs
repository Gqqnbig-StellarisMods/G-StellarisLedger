using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddMvc()
					.AddWebApiConventions()
					.AddJsonOptions(options=> options.SerializerSettings.Formatting=Newtonsoft.Json.Formatting.Indented);
			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
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
            app.UseMvc();
        }
    }
}
