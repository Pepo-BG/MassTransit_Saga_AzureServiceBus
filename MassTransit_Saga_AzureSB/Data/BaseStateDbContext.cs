using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using MassTransit_Saga_AzureSB.Saga;
using Microsoft.EntityFrameworkCore;

namespace MassTransit_Saga_AzureSB.Data;

public class BaseStateDbContext : SagaDbContext
{
    public BaseStateDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations => throw new NotImplementedException();

    private void TrackChanges()
    {
        foreach (var entityEntry in ChangeTracker.Entries())
        {
            if (entityEntry.Entity is SqlServerStateInstance entity)
            {
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        {
                            entity.UpdatedOn = DateTime.UtcNow;
                            break;
                        }
                    case EntityState.Modified:
                        {
                            entity.UpdatedOn = DateTime.UtcNow;
                            if (entityEntry.OriginalValues[nameof(entity.PreviousState)]?.ToString() != entity.CurrentState)
                                entity.PreviousState = entity.CurrentState;
                            break;
                        }
                }
            }
        }
    }
    public override int SaveChanges()
    {
        TrackChanges();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        TrackChanges();
        return base.SaveChangesAsync(cancellationToken);
    }
}
