using BankingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Data
{
    public class BankingDbContext: IdentityDbContext<ApplicationUser>
    {

        public BankingDbContext(DbContextOptions<BankingDbContext> options)
       : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //   // Configure the User-Transaction relationship
        //    modelBuilder.Entity<Transaction>()
        //       .HasOne(t => t.User)
        //        .WithMany(u => u.Transactions)
        //        .HasForeignKey(t => t.UserId)
        //        .OnDelete(DeleteBehavior.Cascade); // Cascade delete when a user is deleted
        //    base.OnModelCreating(modelBuilder);

        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
