using System;
using ServiceStack.DataAnnotations;

namespace AppFrame.ServiceInterface.Models
{
	public class BaseModel
	{

		[AutoIncrement]
		public uint Id {
			get;
			set;
		}
		DateTime? createdAt;
		public DateTime CreatedAt {
			get{
				if (createdAt == null)
					createdAt = DateTime.Now;
				return createdAt.Value;
			}
			set{
				createdAt = value;
			}
		}
		/*
		public DateTime? UpdatedAt {
			get;
			set;
		}
		*/
	}
}

