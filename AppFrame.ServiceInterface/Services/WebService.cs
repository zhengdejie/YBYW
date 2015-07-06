using System;
using ServiceStack;
using AppFrame.Common.API;
using ServiceStack.OrmLite;
using AppFrame.ServiceInterface.Models;
using AppFrame.Common.DTO;
using System.Data;


namespace AppFrame.ServiceInterface.Services
{
	[WebAuthAttribute]
	public class WebService : Service
	{

		[DefaultView("home")]
		public object Any(WebHome request){
			return new HttpResult ();
		}
	}
}

