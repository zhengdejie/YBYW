using System;
using ServiceStack;
using AppFrame.Common.DTO;

namespace AppFrame.Common.API
{
	public interface ILogin{

		string Mobile{get;}
		string Email{ get; }
		string Password{get;}
	}
	public interface IRegister{

		string Name{get;}
		string Mobile{get;}
		string Email{ get; }
		string Password{get;}
		string Avatar{get;}
	}
	[Route("/login", "POST")]
	public class Login : IAPIReturn<AuthResult>, ILogin
	{
		public string Mobile{get;set;}
		public string Email{ get; set; }
		public string Password{get;set;}
	}

	[Route("/register", "POST")]
	public class Register : IAPIReturn<AuthResult>, IRegister
	{
		public string Name{get;set;}
		public string Mobile{get;set;}
		public string Email{ get; set; }
		public string Password{get;set;}
		public string Avatar{get;set;}
	}



	[Route("/profile/{Id}", "GET")]
	public class UserProfile : IAPIReturn<UserDetail>
	{
		public int Id{get;set;}
	}

	[Route("/profile/{Id}/update", "POST")]
	public class UpdateUserProfile : IAPIReturn<UserDetail>
	{
		public int Id{get;set;}
		public string Name{get;set;}
		public string Mobile{get;set;}
		public string Email{ get; set; }
		public string Password{get;set;}
		public string Avatar{get;set;}
	}
}

