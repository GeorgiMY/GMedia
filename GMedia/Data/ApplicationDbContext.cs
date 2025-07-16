using GMedia.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GMedia.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public DbSet<Friendship> Friendships { get; set; }

		public DbSet<Invitation> Invitations { get; set; }

		public DbSet<Post> Posts { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<User>(user =>
			{
				user.HasIndex(u => u.UserName).IsUnique();
				user.Property(u => u.UserName).IsRequired().HasMaxLength(50);
				user.HasIndex(u => u.Email).IsUnique();
				user.Property(u => u.Email).IsRequired().HasMaxLength(256);

				user.Property(u => u.Names).IsRequired().HasMaxLength(100);
				user.Property(u => u.Gender).IsRequired();
				user.Property(u => u.BirthDate).IsRequired();
				user.Property(u => u.Visibility).IsRequired();
			});

			builder.Entity<Invitation>(invitation =>
			{
				invitation.HasKey(i => i.Id);
				invitation.HasOne(i => i.CreatorOfInvitation).WithMany().HasForeignKey(i => i.CreatorOfInvitationId).IsRequired();
				invitation.HasOne(i => i.RegisteredUser).WithMany().HasForeignKey(i => i.RegisteredUserId);
			});

			builder.Entity<Friendship>(friendship =>
			{
				friendship.HasKey(fs => fs.Id);
				friendship.HasOne(fs => fs.Sender).WithMany().HasForeignKey(fs => fs.SenderId).IsRequired();
				friendship.HasOne(fs => fs.Responder).WithMany().HasForeignKey(fs => fs.ResponderId);
				friendship.HasOne(fs => fs.WithdrawnBy).WithMany().HasForeignKey(fs => fs.WithdrawnById);
			});

			builder.Entity<Post>(post =>
			{
				post.HasKey(p => p.Id);
				post.HasOne(p => p.Author).WithMany().HasForeignKey(p => p.AuthorId).IsRequired();
				post.Property(p => p.Text).IsRequired().HasMaxLength(500);
			});
		}
	}
}
