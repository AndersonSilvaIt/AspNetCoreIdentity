using AspNetCoreIdentity.Config;
using AspNetCoreIdentity.Extensions;
using KissLog;
using KissLog.Apis.v1.Listeners;
using KissLog.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreIdentity {
	public class Startup {
		public IConfiguration Configuration { get; }
		
		//Vem assim por default
		//public Startup(IConfiguration configuration) {
		//	Configuration = configuration;
		//}

		public Startup(IHostingEnvironment hostingEnvironment) {
			//Configuração para setar o ambiente de desenvolvimento
				var builder = new ConfigurationBuilder()
				.SetBasePath(hostingEnvironment.ContentRootPath)
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true, true)
				.AddEnvironmentVariables();

			if(hostingEnvironment.IsProduction()) {

				/*Copiar a conenction string da produção appsettings.Production.json
				 clicar botão direito no projeto e "Manage User Secrets", colar a connection string nesse arquivo secrets.json
				 */
				builder.AddUserSecrets<Startup>();
			}

			Configuration = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services) {

			services.AddIdentityConfig(Configuration);
			services.AddAuthorizationConfig();
			services.ResolveDependencies();

			services.AddMvc(options => {
				options.Filters.Add(typeof(AuditoriaFilter));

			}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if(env.IsDevelopment()) {

				app.UseDeveloperExceptionPage();

			} else {

				//app.UseExceptionHandler("/Home/Error");

				app.UseExceptionHandler("/erro/500");
				app.UseStatusCodePagesWithRedirects("/erro/{0}");

				app.UseHsts();
			}

			app.UseKissLogMiddleware();
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			//Para autilizar o Identity
			app.UseAuthentication();

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
			 
			LogConfig.RegisterKissLogListeners(Configuration);
		}

	}
}
