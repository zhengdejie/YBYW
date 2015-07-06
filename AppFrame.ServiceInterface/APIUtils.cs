using System;
using ServiceStack;
using ServiceStack.Web;
using AppFrame.Common.DTO;

namespace AppFrame.ServiceInterface
{
	public static class APIUtils
	{
		const string AuthedUserKey = "AuthedUser";
		public const string AuthedUserSessionKey = "SessionUser";

		public static AppFrame.ServiceInterface.Models.User GetAuthedUser(this Service service){
			return GetAuthedUser (service.Request);
		}
		public static AppFrame.ServiceInterface.Models.User GetAuthedUser(this IRequest request){
			if (request.Items.ContainsKey (AuthedUserKey)) {
				return (AppFrame.ServiceInterface.Models.User)request.Items [AuthedUserKey];
			}
			var u = request.GetSessionBag ().Get<AppFrame.ServiceInterface.Models.User> (AuthedUserSessionKey);
			if (u != null) {
				return u;
			}
			return null;
		}
		public static void SetAuthedUser(this IRequest request, AppFrame.ServiceInterface.Models.User u){
			request.Items [AuthedUserKey] = u;
		}

		const string GenericErrorCode = "Error";
		public static APIResult<T> APIResult<T>(this T t, string errorCode, string message, string messageType){
			APIResult<T> r = new APIResult<T> ();
			r.Data = t;
			if (errorCode != null || message != null) {
				r.ResponseStatus = new ResponseStatus ();
				r.ResponseStatus.ErrorCode = errorCode;
				r.ResponseStatus.Message = (String.IsNullOrEmpty(messageType) ? "" : (messageType + "||")) + message;
			}
			return r;
		}
		public static APIResult<object> APIError(string message){
			return APIResult<object> (null, GenericErrorCode, message, "Error");
		}
		public static APIResult<object> APIError(string errorCode, string message){
			return APIResult<object> (null, errorCode, message, "Error");
		}

		public static APIResult<object> APIDone(string message){
			return APIResult<object> (null, null, message, "Info");
		}
		public static APIResult<T> APIDone<T>(this T t){
			return APIResult<T> (t, null, null, null);
		}
		public static APIResult<T> APIDone<T>(this T t, string message){
			return APIResult<T> (t, null, message, "Info");
		}
		public static APIResult<object> APISuccess(string message){
			return APIResult<object> (null, null, message, "Success");
		}
		public static APIResult<T> APISuccess<T>(this T t, string message){
			return APIResult<T> (t, null, message, "Success");
		}
		public static APIResult<object> APIWarning(string message){
			return APIResult<object> (null, null, message, "Warning");
		}
		public static APIResult<T> APIWarning<T>(this T t, string message){
			return APIResult<T> (t, null, message, "Warning");
		}
		public static bool MatchesRegex(this string s, string regex){
			return s != null && System.Text.RegularExpressions.Regex.Match (s, regex).Success;
		}
		public static T MapValuesTo<T>(this object o){
			string json = o.ToJson ();
			return json.FromJson<T> ();
		}
		public static string CheckNotEmpty(this string s){
			return CheckNotEmpty (s, null);
		}
		public static string CheckNotEmpty(this string s, string errorMessage){
			return Check(s, ss => !String.IsNullOrEmpty(ss), errorMessage);
		}
		public static string CheckWithRegex(this string s, string regex){
			return CheckWithRegex (s, null);
		}
		public static string CheckWithRegex(this string s, string regex, string errorMessage){
			return Check(s, ss => ss != null && ss.MatchesRegex(regex), errorMessage);
		}
		public static T Check<T>(this T s, Func<T,bool> f){
			return Check (s, f, null);
		}
		public static T Check<T>(this T s, Func<T,bool> f, string errorMessage){
			if (!f.Invoke (s))
				throw new ArgumentException (errorMessage ?? "参数非法");
			return s;
		}
	}
}

