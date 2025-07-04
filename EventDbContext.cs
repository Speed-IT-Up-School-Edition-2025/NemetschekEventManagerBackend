using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.JSON;
using System.Text.Json;

namespace NemetschekEventManagerBackend
{
    public class EventDbContext : IdentityDbContext<User>
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Submit> Submits { get; set; }

        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                //Email should be unique
                entity.HasIndex(u => u.Email)
                .IsUnique();
            });
                

            modelBuilder.Entity<Event>(entity =>
            {
                //Json serialization for fields
                entity.Property(e => e.Fields)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<IList<Field>>(v!, new JsonSerializerOptions())
                );

                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Submit>(entity =>
            {
                //Composite key for Submit
                entity.HasKey(s => new { s.EventId, s.UserId });

                //Foreign keys for Submit
                entity.HasOne(s => s.Event)
                .WithMany(e => e.Submissions)
                .HasForeignKey(s => s.EventId);

                entity.HasOne(s => s.User)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.UserId);

                //Json serialization for submissions
                entity.Property(e => e.Submissions)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<IList<Submission>>(v!, new JsonSerializerOptions())
                );
            });
                
            base.OnModelCreating(modelBuilder);
        }
    }
}

