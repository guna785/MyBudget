using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Models.Chat;
using MyBudget.Domain.Contract;
using MyBudget.Domain.Entities;

namespace MyBudget.Infrastructure.Contexts
{
    public class ApplicationDbContext : AuditableContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
            : base(options)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public DbSet<ChatHistory<ApplicationUser>> ChatHistories { get; set; }
        public DbSet<Account> Accounts { get; set; }    
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Assets> Assets { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<DebtTransaction> DebtTransactions { get; set; }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<IAuditableEntity>? entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = _dateTimeService.NowUtc;
                        entry.Entity.CreatedBy = _currentUserService.UserName;
                        entry.Entity.IPAddress = _currentUserService.IpAddress;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
                        entry.Entity.LastModifiedBy = _currentUserService.UserName;
                        entry.Entity.IPAddress = _currentUserService.IpAddress;
                        break;
                }
            }
            return _currentUserService.UserId == 0
                ? await base.SaveChangesAsync(cancellationToken)
                : await base.SaveChangesAsync(_currentUserService.UserName, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableProperty? property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableProperty? property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
            {
                property.SetColumnType("nvarchar(128)");
            }

            base.OnModelCreating(builder);
            _ = builder.Entity<ChatHistory<ApplicationUser>>(entity =>
            {
                _ = entity.ToTable("ChatHistory");

                _ = entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.ChatHistoryFromUsers)
                    .HasForeignKey(d => d.FromUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                _ = entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.ChatHistoryToUsers)
                    .HasForeignKey(d => d.ToUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
            _ = builder.Entity<ApplicationUser>(entity =>
            {
                _ = entity.ToTable(name: "Users");
                _ = entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            _ = builder.Entity<ApplicationRole>(entity =>
            {
                _ = entity.ToTable(name: "Roles");
            });
            _ = builder.Entity<IdentityUserRole<int>>(entity =>
            {
                _ = entity.ToTable("UserRoles");
            });

            _ = builder.Entity<IdentityUserClaim<int>>(entity =>
            {
                _ = entity.ToTable("UserClaims");
            });

            _ = builder.Entity<IdentityUserLogin<int>>(entity =>
            {
                _ = entity.ToTable("UserLogins");
            });

            _ = builder.Entity<ApplicationRoleClaim>(entity =>
            {
                _ = entity.ToTable(name: "RoleClaims");

                _ = entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            _ = builder.Entity<IdentityUserToken<int>>(entity =>
            {
                _ = entity.ToTable("UserTokens");
            });
        }
    }
}
