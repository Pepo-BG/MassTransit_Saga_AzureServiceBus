using Automatonymous;

namespace MassTransit_Saga_AzureSB.Saga
{
    public class MyStateInstance : SqlServerStateInstance, SagaStateMachineInstance
    //add any properties different than the base (SqlServerStateInstance) class here!
    {
    }
}
