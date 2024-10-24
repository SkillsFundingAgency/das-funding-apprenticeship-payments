using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess;

public class ApprenticeshipPaymentsDataContext : DbContext
{
    public ApprenticeshipPaymentsDataContext(DbContextOptions<ApprenticeshipPaymentsDataContext> options) : base(options)
    {
    }

    public virtual DbSet<Apprenticeship> Apprenticeships { get; set; }
    public virtual DbSet<Earning> Earnings { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apprenticeship
        modelBuilder.Entity<Apprenticeship>()
            .HasMany(x => x.Earnings)
            .WithOne()
            .HasForeignKey(fk => fk.ApprenticeshipKey);
        modelBuilder.Entity<Apprenticeship>()
            .HasMany(x => x.Payments)
            .WithOne()
            .HasForeignKey(fk => fk.ApprenticeshipKey);
        modelBuilder.Entity<Apprenticeship>()
            .HasKey(a => new { a.ApprenticeshipKey });
        modelBuilder.Entity<Apprenticeship>()
            .Property(p => p.EmployerType)
            .HasConversion(
                v => v.ToString(),
                v => (EmployerType)Enum.Parse(typeof(EmployerType), v));

        // Earning
        modelBuilder.Entity<Earning>()
            .HasKey(a => new { a.Key });

        // Payment
        modelBuilder.Entity<Payment>()
            .HasKey(a => new { a.Key });

        base.OnModelCreating(modelBuilder);
    }
}