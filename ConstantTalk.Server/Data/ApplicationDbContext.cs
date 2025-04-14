using ConstantTalk.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstantTalk.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<SubscriberService> SubscriberServices { get; set; }
        public DbSet<Bill> Bills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Subscriber>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Subscriber>()
                .HasIndex(s => s.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<SubscriberService>()
                .HasIndex(ss => new { ss.SubscriberId, ss.ServiceId })
                .IsUnique();
        }
    }
}
