using System;
using ServiceStack;
using AppFrame.Common.DTO;
namespace AppFrame.Common.API
{
	[Route("/home", "GET")]
	public class WebHome{

	}
	[Route("/weblogin", "GET")]
	public class WebLoginGet{
		
	}
	[Route("/webregister", "GET")]
	public class WebRegisterGet{

	}

	[Route("/weblogin", "POST")]
	public class WebLoginPost : IReturn<HttpResult>, ILogin
	{
		public string Mobile{get;set;}
		public string Email{ get; set; }
		public string Password{get;set;}
	}

	[Route("/webregister", "POST")]
	public class WebRegisterPost : IReturn<HttpResult>, IRegister

	{
		public string Name{get;set;}
		public string Mobile{get;set;}
		public string Email{ get; set; }
		public string Password{get;set;}
		public string Avatar{get;set;}
	}


	[Route("/weblogout")]
	public class WebLogout : IReturn<HttpResult>
	{
	}
}

