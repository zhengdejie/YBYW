using System;

namespace AppFrame.Common.DTO
{
	public class AuthResult
	{
		public string Token{get;set;}
		public UserDetail User{ get; set; }
	}

	public class UserBrief{
		public int Id{get;set;}
		public string Name{get;set;}
		public string Avatar{ get; set; }
		public DateTime CreatedAt{ get; set;}

		public DateTime? UpdatedAt{ get; set;}
	}
	public class UserDetail : UserBrief{
		
	}
}

