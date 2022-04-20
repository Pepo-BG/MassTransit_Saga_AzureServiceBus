using Automatonymous;

namespace MassTransit_Saga_AzureSB.Saga;

public abstract class SqlServerStateInstance : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public string? PreviousState { get; set; }
    public byte[] RowVersion { get; set; }
    public DateTime UpdatedOn { get; set; }
}

