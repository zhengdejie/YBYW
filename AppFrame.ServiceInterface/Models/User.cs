using System;
using ServiceStack.Auth;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using System.Security.Cryptography;

namespace AppFrame.ServiceInterface.Models
{
	public class User : BaseModel
	{
		
		public string Name {
			get;
			set;
		}

		public string Password {
			get;
			set;
		}
		public string Email {
			get;
			set;
		}
		public string Mobile {
			get;
			set;
		}


		public string Avatar {
			get;
			set;
		}

		public string AuthToken {
			get;
			set;
		}
		public static int GetIdFromAuthToken(string authToken){
			if (authToken == null)
				return 0;
			string p1 = authToken.Split ('-') [0];
			int result = 0;
			int.TryParse (p1, out result);
			return result;
		}
		public string GenerateAuthToken(){
			return Id + "-" + System.Guid.NewGuid().ToString().Replace("-", "");
		}
		public static string HashPassword(string rawPassword){
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(rawPassword);
			SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
			return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
		}
	}
}

