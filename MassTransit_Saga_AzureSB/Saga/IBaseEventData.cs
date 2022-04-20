using Automatonymous;

namespace MassTransit_Saga_AzureSB.Saga;

public interface IBaseEventData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
}
