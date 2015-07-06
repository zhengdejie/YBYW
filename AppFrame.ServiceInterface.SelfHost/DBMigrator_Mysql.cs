using System;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
namespace AppFrame.ServiceInterface
{
	public class DBMigrator_Mysql
	{
		string connectionString;
		Assembly migrationAssembly;

		public DBMigrator_Mysql(string connectionString, Assembly migrationAssembly)
		{
			this.connectionString = connectionString;
			this.migrationAssembly = migrationAssembly;
		}

		private class MigrationOptions : IMigrationProcessorOptions
		{
			public bool PreviewOnly { get; set; }
			public int Timeout { get; set; }
			public string ProviderSwitches{ get; set; }
		}
		public void MigrateToZero(){
			Migrate(runner => runner.MigrateDown(0));
		}
		public void MigrateUp(){

			Migrate(runner => runner.MigrateUp());
		}
		private void Migrate(Action<IMigrationRunner> runnerAction)
		{
			var options = new MigrationOptions { PreviewOnly = false, Timeout = 0 };
			var factory = new FluentMigrator.Runner.Processors.MySql.MySqlProcessorFactory();

			//using (var announcer = new NullAnnouncer())
			var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));
			var migrationContext = new RunnerContext(announcer)
			{
				#if DEBUG
				// will create testdata
				Profile = "development"
				#endif
			};
			var processor = factory.Create(this.connectionString, announcer, options);
			var runner = new MigrationRunner(migrationAssembly, migrationContext, processor);
			runnerAction(runner);
		}
	}
}

