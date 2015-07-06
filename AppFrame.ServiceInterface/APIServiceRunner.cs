using System;
using ServiceStack.Host;
using ServiceStack;
using ServiceStack.Web;

namespace AppFrame.ServiceInterface
{
	/// <summary>
	/// API service runner.
	/// 
	/// 作用：
	/// 1. 计算并输出每个api的执行时间，用来以后debug／分析
	/// </summary>
	public class APIServiceRunner<T> : ServiceRunner<T> 
	{
		public APIServiceRunner (IAppHost appHost, ActionContext actionContext) : base(appHost, actionContext){
		}
		public override void OnBeforeExecute (IRequest requestContext, T request){
			requestContext.Items ["ExecutionBegin"] = DateTime.Now.Ticks;

		}
		public override object OnAfterExecute(
			IRequest requestContext, object response) 
		{
			var result = base.OnAfterExecute (requestContext, response);
			if (response == null)
				return result;
			Type t = response.GetType ();
			long executionBegin = (long)requestContext.Items ["ExecutionBegin"];
			TimeSpan timeUsed = new TimeSpan(DateTime.Now.Ticks - executionBegin);

			if (t.IsGenericType && t.GetGenericTypeDefinition () == typeof(AppFrame.Common.DTO.APIResult<>)) {
				t.GetProperty ("ExecutionTime").SetValue (response, (int)timeUsed.TotalMilliseconds);
			}
			return result;
		}
	}
}

