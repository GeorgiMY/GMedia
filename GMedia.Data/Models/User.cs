using Microsoft.AspNetCore.Identity;
using static GMedia.Data.Enums;

namespace GMedia.Data.Models
{
	public class User : IdentityUser
	{
		public string Names { get; set; }

		public Gender Gender { get; set; }

		public DateTime BirthDate { get; set; }

		public VisibilityOptions Visibility { get; set; }
	}
}
