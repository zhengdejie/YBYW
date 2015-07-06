using System;
using ServiceStack;
namespace AppFrame.Common.DTO
{
	
	public interface IAPIReturn : IReturn<APIResult<object>>{

	}
	public interface IAPIReturn<T> : IReturn<APIResult<T>>{
		
	}
	public class APIResult<T>{
		public T Data{get;set;}
		public ResponseStatus ResponseStatus { get; set; } 

		public int ExecutionTime {get;set;}
	}


}

