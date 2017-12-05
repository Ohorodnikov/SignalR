using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using Microsoft.EntityFrameworkCore;
using SignalR.Models;

namespace SignalR.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ImageModel> ImageModel { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<SessionUser> SessionUser { get; set; }
        public DbSet<VoteInfo> VoteInfo { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ImageModel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Base64);

                entity.Property(e => e.Description);
                
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImageModel_AspNetUsers_ApplicationUserId");
            });

            builder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            builder.Entity<SessionUser>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ConnectionId);
                entity.Property(e => e.IsTeacher);

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SessionUser_Session");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SessionUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SessionUser_ApplicationUser");
            });

            builder.Entity<VoteInfo>(entity =>
            {
                entity.HasKey(t => new { t.UserId, t.ImageId, t.SessionId });
                               
               
            });
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
