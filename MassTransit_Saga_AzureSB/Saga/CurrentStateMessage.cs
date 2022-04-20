namespace MassTransit_Saga_AzureSB.Saga;

public class CurrentStateMessage
{
    public Guid CorrelationID { get; set; }
    public string State { get; set; }
    public bool IsStateUpdated { get; set; }
}
