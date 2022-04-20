using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using MassTransit_Saga_AzureSB.Saga;
using Microsoft.EntityFrameworkCore;

namespace MassTransit_Saga_AzureSB.Data;

public class StateDbContext : BaseStateDbContext
{
    public StateDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyStateInstance>().ToTable<MyStateInstance>(ba => ba.IsTemporal());
        base.OnModelCreating(modelBuilder);
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new SagaStateMap<MyStateInstance>(); }
    }
}
