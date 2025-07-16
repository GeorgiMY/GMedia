using static GMedia.Enums;

namespace GMedia.Models
{
	public class Friendship
	{
		public int Id { get; set; }

		public DateTime SentAt { get; set; } = DateTime.Now;

		public string SenderId { get; set; }

		public User Sender { get; set; }

		public DateTime? RespondedAt { get; set; }

		public string? ResponderId { get; set; }

		public User? Responder { get; set; }

		public DateTime? WithdrawnAt { get; set; }

		public string? WithdrawnById { get; set; }

		public User? WithdrawnBy { get; set; }

		public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
	}
}
