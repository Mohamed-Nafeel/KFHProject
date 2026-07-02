using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace KFH
{
    public class KFHContext : DbContext
    {

        public KFHContext(DbContextOptions<KFHContext> options) : base(options)
        {

        }

        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<TransferRequest> TransferRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Basic CustomerAccount mapping
            modelBuilder.Entity<CustomerAccount>().HasKey(e => e.AccountNumber);
            modelBuilder.Entity<CustomerAccount>().Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<CustomerAccount>().Property(e => e.Status).IsRequired().HasMaxLength(50);

            // Basic TransferRequest mapping
            modelBuilder.Entity<TransferRequest>().HasKey(e => new { e.SourceAccount, e.DestinationAccount, e.CreatedDate });
            modelBuilder.Entity<TransferRequest>().Property(e => e.Amount).IsRequired();
            modelBuilder.Entity<TransferRequest>().Property(e => e.Status).IsRequired().HasMaxLength(50);

            // Configure relationships to CustomerAccount (no navigation properties)
            modelBuilder.Entity<TransferRequest>()
                .HasOne<CustomerAccount>()
                .WithMany()
                .HasForeignKey(e => e.SourceAccount)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferRequest>()
                .HasOne<CustomerAccount>()
                .WithMany()
                .HasForeignKey(e => e.DestinationAccount)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

