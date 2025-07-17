using static GMedia.Data.Enums;

namespace GMedia.Data.Models
{
	public class Post
	{
		public Guid Id { get; set; }

		public string AuthorId { get; set; }

		public User Author { get; set; }

		public string Text { get; set; }

		public VisibilityOptions Visibility { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}
