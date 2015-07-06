using System;
using ServiceStack;
using ServiceStack.Web;
using System.Net;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Caching;
using AppFrame.ServiceInterface.Models;

namespace AppFrame.ServiceInterface
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class APIAuthAttribute : RequestFilterAttribute
	{
		public IDbConnectionFactory DbFactory { get; set; }
		public ICacheClient CacheClient { get; set; }

		public override void Execute(IRequest req, IResponse res, object requestDto)
		{
			var user = req.GetAuthedUser ();
			if (user != null)
				return;

			var auth = req.Headers ["Authorization"];
			if (String.IsNullOrEmpty(auth)) {
				failOnNotAuthed (req, res, "no authorization header");
				return;
			}
			var methodAndToken = auth.Split (' ');
			if (methodAndToken.Length != 2) {
				failOnNotAuthed (req, res, "authorization format error: " + auth);
				return;
			}
			switch (methodAndToken [0]) {
			case "AppFrame":
				// Authorization: AppFrame UserId:UserAuthToken
				int id = User.GetIdFromAuthToken (methodAndToken [1]);
				if (id == 0) {
					failOnNotAuthed (req, res, "authorization id error: " + auth);
					return;
				}
				using (var db = DbFactory.OpenDbConnection ()) {
					user = db.SingleById<User> (id);
					if(user == null || user.AuthToken != methodAndToken[1]){
						failOnNotAuthed (req, res, "AppFrame authorization failed");
						return;
					}
					req.SetAuthedUser( user);
				}
				break;
			default:
				failOnNotAuthed (req, res, "unknown authorization method: " + methodAndToken [0]);
				return;
			}
		}

		void failOnNotAuthed(IRequest req, IResponse res, string reason)
		{

			if (req.ResponseContentType.MatchesContentType (MimeTypes.Json)) {
				res.StatusCode = (int)HttpStatusCode.OK;
				var result = new AppFrame.Common.DTO.APIResult<object> () {
					ResponseStatus = new ResponseStatus(){
						ErrorCode = HttpStatusCode.Unauthorized.ToString(),
						Message = reason
					}
				};
				var b = res.WriteToResponse (result, MimeTypes.Json).Result;
			} else {
				res.StatusCode = (int)HttpStatusCode.Unauthorized;
				res.Write ("not authorized: " + reason);
			}
			res.Close();
		}
	}
}

