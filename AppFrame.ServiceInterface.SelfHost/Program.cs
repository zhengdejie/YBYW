using System;
using ServiceStack;
using System.Threading.Tasks;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;
using System.Configuration;
using ServiceStack.DataAnnotations;
using ServiceStack.Auth;
using AppFrame.ServiceInterface.Models;
using ServiceStack.Caching;
using ServiceStack.Web;
using ServiceStack.Host;
using AppFrame.ServiceInterface.Services;
using System.IO;
using System.Reflection;
namespace AppFrame.ServiceInterface.SelfHost
{
	class MainClass
	{
		const string DBConnectionConfigPrefix = "DBConnection:Dev:";
#if DEBUG
		const string DBConnectionConfigDefault = "DBConnection:Debug";
#else
		const string DBConnectionConfigDefault = "DBConnection:Release";
#endif
		//Define the Web Services AppHost
		public class AppHost : AppSelfHostBase {
			public AppHost() 
				: base("HttpListener Self-Host", ServiceUtils.GetServiceAssembly()) {}

			public override void Configure(Funq.Container container) {
				Plugins.Add(new ServiceStack.Razor.RazorFormat());
				Plugins.Add(new SessionFeature());

				var machineName = System.Environment.MachineName;
				// Cache
				container.Register<ICacheClient>( new MemoryCacheClient() );

				// DB
				string mysqlConnectionString = null;
				foreach (var k in ConfigurationManager.AppSettings.AllKeys) {
					if (k.StartsWith (DBConnectionConfigPrefix)) {
						String regex = k.Substring (DBConnectionConfigPrefix.Length);
						if (machineName.MatchesRegex (regex)) {
							mysqlConnectionString = ConfigurationManager.AppSettings[k];
							break;
						}
					}
				}
				if (mysqlConnectionString == null)
					mysqlConnectionString = ConfigurationManager.AppSettings [DBConnectionConfigDefault];

				if(mysqlConnectionString == null)
					throw new NotImplementedException("请自己在App.config里配置本机的connection string");

				// DB migration
				DBMigrator_Mysql migrator = new DBMigrator_Mysql (mysqlConnectionString, ServiceUtils.GetDBMigrationAssembly());
				if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("ResetDB"))) {
					migrator.MigrateToZero ();
				}
				migrator.MigrateUp();


				var dbFactory = new OrmLiteConnectionFactory (mysqlConnectionString, MySqlDialectProvider.Instance);

				container.Register<IDbConnectionFactory>(dbFactory);

				// 文件夹结构：
				// Web
				// AppFrame.ServiceInterface.SelfHost
				//  bin
				//    Debug
				//      .exe

				string exePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
				SetConfig(new HostConfig {
					// https://github.com/ServiceStack/ServiceStack/wiki/Error-Handling
					MapExceptionToStatusCode = {
						
						{ typeof( ArgumentException ), 200 },
						{ typeof( Exception ), 200 },

					},
					DebugMode = true, //Enable StackTraces in development
					WebHostPhysicalPath = Path.Combine(Path.GetDirectoryName(exePath), "..", "..", "..", "Web")
				});

				ServiceUtils.ConfigureService (this, container);

			}
			public override IServiceRunner<TRequest> CreateServiceRunner<TRequest>(
				ActionContext actionContext)
			{           
				return new APIServiceRunner<TRequest>(this, actionContext); 
			}
		}


		//Run it!
		static void Main(string[] args)
		{
			// crack it
			var field = typeof(LicenseUtils).GetField("__activatedLicense", 
				BindingFlags.Static | 
				BindingFlags.NonPublic);
			// https://github.com/ServiceStack/ServiceStack.Text/blob/12aa7ea0c532be1d648768c8bef1487b10839fb7/src/ServiceStack.Text/LicenseUtils.cs
			field.SetValue(null, new LicenseKey(){
				Type = LicenseType.Enterprise
			});
			

			var listeningOn = args.Length == 0 ? "http://*:1337/" : args[0];
			new AppHost()
				.Init()
				.Start(listeningOn);

			Console.WriteLine("AppHost Created at {0}, listening on {1}", 
				DateTime.Now, listeningOn);

			Console.ReadKey();
		}
	}
}
