namespace GMedia.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatorOfInvitationId { get; set; }
        public User CreatorOfInvitation { get; set; }
        public string Email { get; set; }
        public DateTime ExpirationDate { get; set; } = DateTime.Now.AddHours(24);
        public string? RegisteredUserId { get; set; }
        public User? RegisteredUser { get; set; }
        public string Code { get; set; } = Guid.NewGuid().ToString();
    }
}
