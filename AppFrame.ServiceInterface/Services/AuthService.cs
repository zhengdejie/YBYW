using System;
using ServiceStack;
using AppFrame.Common.API;
using ServiceStack.OrmLite;
using AppFrame.ServiceInterface.Models;
using AppFrame.Common.DTO;
using System.Data;

namespace AppFrame.ServiceInterface.Services
{
	
	public class AuthService : Service
	{
		protected User DoLogin(ILogin request, bool resetAuthToken){
			User user;
			if (!String.IsNullOrEmpty (request.Mobile)) {
				user = Db.Single<User> (u => 
					u.Mobile == request.Mobile && u.Password == User.HashPassword(request.Password));
				if(user == null) throw new ArgumentException ("手机号或者密码错误");
			} else if (!String.IsNullOrEmpty (request.Email)) {

				user = Db.Single<User> (u => 
					u.Email == request.Email && u.Password == User.HashPassword(request.Password));
				if(user == null) throw new ArgumentException ("邮箱或者密码错误");
			} else {
				throw new ArgumentException ("手机 & Email 不能都为空");
			}
			if (resetAuthToken) {
				user.AuthToken = user.GenerateAuthToken ();
				if (Db.Update<User> (new { AuthToken = user.AuthToken }, u => u.Id == user.Id) == 0)
					throw new Exception ("登陆失败");
			}
			return user;
		}


		protected User DoRegister(IRegister request){

			User user = new User () {
				Name = request.Name.CheckNotEmpty("必须输入昵称"),
				Password = User.HashPassword(request.Password.CheckWithRegex("^.{6,}$", "密码必须为6位以上")),
				Mobile = request.Mobile.Check(s => s == null || s.MatchesRegex("^\\d{11}$"), "手机的格式不正确"),
				Email = request.Email.Check(s => s == null || s.MatchesRegex("^.*\\@.*\\..*$"), "邮箱的格式不正确"),
				Avatar = request.Avatar
			};

			using (IDbTransaction trans = Db.OpenTransaction())
			{
				if (String.IsNullOrEmpty (user.Email) && String.IsNullOrEmpty (user.Mobile))
					throw new ArgumentException ("手机 & Email 不能都为空");
				if (!String.IsNullOrEmpty (user.Mobile) && Db.Exists<User> (u => u.Mobile == user.Mobile))
					throw new Exception ("手机已被注册");
				if (!String.IsNullOrEmpty (user.Email) && Db.Exists<User> (u => u.Email == user.Email))
					throw new Exception ("邮箱已被注册");

				user.AuthToken = "Registering";
				if(!Db.Save(user))
					throw new Exception ("注册失败");

				user.AuthToken = user.GenerateAuthToken ();
				if(Db.Update<User>(new { AuthToken = user.AuthToken }, p => p.Id == user.Id) <= 0)
					throw new Exception ("更新AuthToken失败");


				trans.Commit ();

				return user;
			}
		}
	}
}

