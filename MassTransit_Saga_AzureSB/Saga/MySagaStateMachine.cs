using Automatonymous;
using MassTransit;

namespace MassTransit_Saga_AzureSB.Saga;

public partial class MySagaStateMachine: MassTransitStateMachine<MyStateInstance>
{
    private readonly ILogger<MySagaStateMachine> _logger;

    public MySagaStateMachine(ILogger<MySagaStateMachine> logger)
    {
        _logger = logger;

        InstanceState(x => x.CurrentState);
        ConfigureCorrelationIds();

        Initially(
            When(StateInitiate)
                .TransitionTo(EmailCollection)
                .RespondAsync(async z => await RespondCurrentState(z)),
            When(CollectEmail)
                .IfElse(c => !c.Data.ActiveAccountExists,
                    then => then.TransitionTo(EmailValidation)
                        .Then(async act =>
                        {
                            _logger.LogWarning("This is example activity!");
                        })
                    ,
                    els => els.TransitionTo(TODONOTIMPLEMENTEDSTATE)
                )
                .RespondAsync(async z => await RespondCurrentState(z))
        );

        DuringAny(
            When(RequestState)
                .Respond(z => new CurrentStateMessage
                {
                    CorrelationID = z.CorrelationId.GetValueOrDefault(),
                    State = z.Instance.CurrentState,
                    IsStateUpdated = true
                })
        );

        During(EmailCollection,
            When(CollectEmail)
                .IfElse(c => !c.Data.ActiveAccountExists,
                    then => then.TransitionTo(EmailValidation)
                        .Then(async act =>
                        {
                            _logger.LogWarning("This is example activity!");
                        })
                    ,
                    els => els.TransitionTo(TODONOTIMPLEMENTEDSTATE)
                )
                .RespondAsync(async z => await RespondCurrentState(z))
        );
    }

    private void ConfigureCorrelationIds()
    {
        Event(() => RequestState, x => { x.CorrelateById(t => t.Message.CorrelationId); x.ReadOnly = true; });
        Event(() => StateInitiate, x => { x.CorrelateById(t => t.Message.CorrelationId); });
        Event(() => CollectEmail, x => { x.CorrelateById(t => t.Message.CorrelationId); });
    }
    private async Task<CurrentStateMessage> RespondCurrentState(ConsumeEventContext<MyStateInstance, IBaseEventData> z)
    {
        return new CurrentStateMessage
        {
            CorrelationID = z.CorrelationId.GetValueOrDefault(),
            State = z.Instance.CurrentState,
            IsStateUpdated = true
        };
    }

    private async Task<CurrentStateMessage> RespondCurrentState(ConsumeEventContext<MyStateInstance, MyStateInstance> z)
    {
        return await z.Init<CurrentStateMessage>(new
        {
            CorrelationID = z.CorrelationId.GetValueOrDefault(),
            State = z.Instance.CurrentState
        });
    }
    public State TODONOTIMPLEMENTEDSTATE { get; set; }
    public State EmailCollection { get; set; }
    public State EmailValidation { get; set; }

    public Event<IBaseEventData> RequestState { get; set; }
    public Event<MyStateInstance> StateInitiate { get; set; }
    public Event<CollectEmailData> CollectEmail { get; set; }
}
