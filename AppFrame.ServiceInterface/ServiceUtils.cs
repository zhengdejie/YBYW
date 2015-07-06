using System;
using ServiceStack;
using System.Reflection;
using ServiceStack.Data;
using ServiceStack.OrmLite;


namespace AppFrame.ServiceInterface
{
	public static class ServiceUtils
	{
		/// <summary>
		/// 获得 Service 类所在的 assembly
		/// </summary>
		public static Assembly GetServiceAssembly(){
			return typeof(ServiceUtils).Assembly;
		}

		/// <summary>
		/// 获得 DBMigration 类所在的 assembly
		/// </summary>
		public static Assembly GetDBMigrationAssembly(){
			return Assembly.GetExecutingAssembly();
		}
		public static void ConfigureService(ServiceStackHost host, Funq.Container container){
			
		}

	}
}

