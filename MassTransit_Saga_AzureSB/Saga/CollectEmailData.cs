namespace MassTransit_Saga_AzureSB.Saga;

public interface CollectEmailData : IBaseEventData
{
    bool ActiveAccountExists { get; }
}