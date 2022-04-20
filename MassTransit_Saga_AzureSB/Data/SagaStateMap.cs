using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using MassTransit_Saga_AzureSB.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransit_Saga_AzureSB.Data;

public class SagaStateMap<TInstance> : SagaClassMap<TInstance> where TInstance : SqlServerStateInstance
{
    protected override void Configure(EntityTypeBuilder<TInstance> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.PreviousState).HasMaxLength(64);

        // If using Optimistic concurrency, otherwise remove this property
        entity.Property(x => x.RowVersion).IsRowVersion();
    }
}
