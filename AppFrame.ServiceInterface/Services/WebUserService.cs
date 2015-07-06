using System;
using ServiceStack;
using AppFrame.Common.API;
using ServiceStack.OrmLite;
using AppFrame.ServiceInterface.Models;
using AppFrame.Common.DTO;
using System.Data;


namespace AppFrame.ServiceInterface.Services
{
	public class WebUserService : AuthService
	{

		[DefaultView("login")]
		public object Any(AppFrame.Common.API.WebLoginGet request){
			return new HttpResult ();
		}

		[DefaultView("register")]
		public object Any(AppFrame.Common.API.WebRegisterGet request){
			return new HttpResult ();
		}
		[DefaultView("login")]
		public object Any(AppFrame.Common.API.WebLoginPost request){
			try{
				var result = DoLogin(request, false);
				SessionBag.Set (APIUtils.AuthedUserSessionKey, result);
				return new HttpResult (new WebPageStatus () {
					RedirectUrl = "/",
					Message = "登入成功",
					MessageType = WebPageStatus.MessageTypeSuccess
				}){
					View = "redirect"
				};
			}catch(Exception e){
				return new HttpResult (new WebPageStatus () {
					Message = e.Message,
					MessageType = WebPageStatus.MessageTypeError
				});
			}
		}

		[DefaultView("register")]
		public object Any(AppFrame.Common.API.WebRegisterPost request){
			try{
				var result = DoRegister(request);
				SessionBag.Set (APIUtils.AuthedUserSessionKey, result);

				return new HttpResult (new WebPageStatus () {
					RedirectUrl = "/",
					Message = "注册成功",
					MessageType = WebPageStatus.MessageTypeSuccess
				}){
					View = "redirect"
				};
			}catch(Exception e){

				return new HttpResult (new WebPageStatus () {
					Message = e.Message,
					MessageType = WebPageStatus.MessageTypeError
				})
					;
			}
		}

		[WebAuth]
		public object Any(AppFrame.Common.API.WebLogout request){
			Request.SetAuthedUser (null);
			SessionBag.Remove (APIUtils.AuthedUserSessionKey);
			return new HttpResult (new WebPageStatus () {
				RedirectUrl = "/",
				Message = "退出成功",
				MessageType = WebPageStatus.MessageTypeSuccess
			}){
				View = "redirect"
			};
		}
	}
}

