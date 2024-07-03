using MeetApp.Infrastructure.Commons.Abstracts;
using MeetApp.Infrastructure.Entities.Membership;
using MeetApp.Infrastructure.Services.Abstracts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeetApp.Data.DataContexts
{
    public class DataContext : IdentityDbContext
        <MeetAppUser, MeetAppRole, int,
        MeetAppUserClaim, MeetAppUserRole,
        MeetAppUserLogin, MeetAppRoleClaim,
        MeetAppUserToken>
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly IIdentityService _identityService;

        public DataContext(
            DbContextOptions options,
            IDateTimeService dateTimeService,
            IIdentityService identityService
            ) : base(options)
        {
            _dateTimeService = dateTimeService;
            _identityService = identityService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }

        public override int SaveChanges()
        {
            var changes = this.ChangeTracker.Entries<IAuditableEntity>();

            if (changes != null)
            {
                foreach (var entity in changes
                    .Where(ch => ch.State == EntityState.Added ||
                    ch.State == EntityState.Deleted ||
                    ch.State == EntityState.Modified))
                {
                    switch (entity.State)
                    {
                        case EntityState.Added:
                            entity.Entity.CreatedAt = _dateTimeService.ExecutingTime;
                            entity.Entity.CreatedBy = _identityService.GetPrincipialId();
                            break;
                        case EntityState.Modified:
                            entity.Entity.ModifiedAt = _dateTimeService.ExecutingTime;
                            entity.Entity.ModifiedBy = _identityService.GetPrincipialId();

                            entity.Property(m => m.CreatedAt).IsModified = false;
                            entity.Property(m => m.CreatedBy).IsModified = false;
                            break;
                        case EntityState.Deleted:
                            entity.State = EntityState.Modified;
                            entity.Entity.DeletedAt = _dateTimeService.ExecutingTime;
                            entity.Entity.DeletedBy = _identityService.GetPrincipialId();

                            entity.Property(m => m.CreatedAt).IsModified = false;
                            entity.Property(m => m.CreatedBy).IsModified = false;
                            entity.Property(m => m.ModifiedBy).IsModified = false;
                            entity.Property(m => m.ModifiedAt).IsModified = false;
                            break;
                        default:
                            break;
                    }
                }
            }

            return base.SaveChanges();
        }

    }
}
