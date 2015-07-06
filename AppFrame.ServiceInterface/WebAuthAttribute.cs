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
	public class WebAuthAttribute : RequestFilterAttribute
	{
		public bool AuthRequired;

		public IDbConnectionFactory DbFactory { get; set; }
		public ICacheClient CacheClient { get; set; }

		public override void Execute(IRequest req, IResponse res, object requestDto)
		{
			var user = req.GetAuthedUser ();
			if (user != null)
				return;
			if (user == null && AuthRequired) {
				failOnNotAuthed (req, res, "not logged in");
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

