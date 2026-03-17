using CommonCore.Entities.BaseEntity;
using core_backend.Common.Entities.DbEntities;
using core_backend.Common.Entities.Identities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace core_backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,
    ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {


        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(IDeleted).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter();
                }
            }

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            builder.Entity<Transaction>(transaction =>
            {
                transaction.HasOne(t => t.Account)
                    .WithMany()
                    .HasForeignKey(t => t.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);

                transaction.HasOne(t => t.ToAccount)
                    .WithMany()
                    .HasForeignKey(t => t.ToAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public async Task<int> SaveChangesAsync(bool populatedICreated = true, bool populatedIModified = true, CancellationToken cancellationToken = default)
        => await SaveChangesInternalAsync(populatedICreated, populatedIModified, cancellationToken);

        private async Task<int> SaveChangesInternalAsync(bool populatedICreated, bool populatedIModified, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;

            var tracked = ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged).ToList();
            foreach (var change in tracked)
            {
                switch (change.State)
                {
                    case EntityState.Added:
                        if (change.Entity is ICreated c && populatedICreated)
                        {
                            c.DateCreated = now;
                            c.DateModified = now;
                        }
                        break;

                    case EntityState.Modified:
                        if (change.Entity is IModified m && populatedIModified)
                        {
                            m.DateModified = now;
                        }
                        break;

                    case EntityState.Deleted:
                        if (change.Entity is IDeleted d)
                        {
                            change.State = EntityState.Modified;
                            d.Deleted = true;
                            d.DateDeleted = now;
                        }
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }

}
