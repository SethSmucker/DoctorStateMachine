namespace DefaultNamespace;

public abstract class State<TActor> : IState<TActor> where TActor : class
{
    protected StateMachine<TActor> StateMachine { get; private set; }
    protected TActor Actor { get; private set; }

    public void Enter(StateMachine<TActor> stateMachine, TActor actor) => OnEnter(stateMachine, actor);
    public void Update()      => OnUpdate();
    public void FixedUpdate() => OnFixedUpdate();
    public void Exit()        => OnExit();

    protected virtual void OnEnter(StateMachine<TActor> stateMachine, TActor actor)
    {
        StateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        Actor = actor ?? throw new ArgumentNullException(nameof(actor));
    }
    protected virtual void OnUpdate() { }
    protected virtual void OnFixedUpdate() { }
    protected virtual void OnExit() { }
}