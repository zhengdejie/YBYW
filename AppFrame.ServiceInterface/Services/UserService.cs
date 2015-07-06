using System;
using ServiceStack;
using AppFrame.Common.API;
using ServiceStack.OrmLite;
using AppFrame.ServiceInterface.Models;
using AppFrame.Common.DTO;
using System.Data;

namespace AppFrame.ServiceInterface.Services
{
	public class APIAuthService : AuthService
	{
		
		public object Any(AppFrame.Common.API.Login request){
			var user = DoLogin (request, true);
			return new AuthResult () {
				Token = user.AuthToken,
				User = user.MapValuesTo<AppFrame.Common.DTO.UserDetail> ()
			}.APISuccess("登陆成功");
		}

		public object Any(AppFrame.Common.API.Register request){
			var user = DoRegister (request);
			return new AuthResult () {
				Token = user.AuthToken,
				User = user.MapValuesTo<AppFrame.Common.DTO.UserDetail> ()
			}.APISuccess("注册成功");
		}

	}

	[APIAuth]
	public class UserService : Service
	{
		public object Any(UserProfile request){
			
			User u = Db.SingleById<User>(request.Id);
			if(u == null) return APIUtils.APIError ("找不到用户");
			return u.MapValuesTo<AppFrame.Common.DTO.UserDetail>().APIDone
			();
		}

		public object Any(UpdateUserProfile request){

			using (IDbTransaction trans = Db.OpenTransaction ()) {
				User u = Db.SingleById<User> (request.Id);
				if (u == null)
					return APIUtils.APIError ("找不到用户");

				if (!string.IsNullOrEmpty (request.Name))
					u.Name = request.Name;
				if (!string.IsNullOrEmpty (request.Mobile))
					u.Mobile = request.Mobile;
				if (!string.IsNullOrEmpty (request.Email))
					u.Email = request.Email;
				if (!string.IsNullOrEmpty (request.Avatar))
					u.Avatar = request.Avatar;
				if (!string.IsNullOrEmpty (request.Password))
					u.Password = request.Password;

				Db.Save (u);
				trans.Commit ();

				return u.MapValuesTo<AppFrame.Common.DTO.UserDetail>().APISuccess("修改设置成功");
			}
		}
	}
}

