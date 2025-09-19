
public class StateMachine<TActor> where TActor : class
{
    public TActor Actor { get; private set; }
    public IState<TActor> CurrentState { get; private set; }

    public event Action<IState<TActor>, IState<TActor>> OnStateChanged;

    private bool _initialized;
    private bool _isTransitioning;

    public void Init(IState<TActor> startState, TActor actor)
    {
        if (_initialized) throw new InvalidOperationException("Init called more than once.");
        Actor = actor ?? throw new ArgumentNullException(nameof(actor));
        _initialized = true;
        TransitionTo(startState);
    }

    public void TransitionTo(IState<TActor> next)
    {
        if (next is null) throw new ArgumentNullException(nameof(next));
        if (ReferenceEquals(CurrentState, next)) return;
        if (_isTransitioning) throw new InvalidOperationException("Re-entrant TransitionTo detected.");

        _isTransitioning = true;
        try
        {
            var prev = CurrentState;
            prev?.Exit();
            CurrentState = next;
            CurrentState.Enter(this, Actor);
            OnStateChanged?.Invoke(prev, CurrentState);
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    public void Update()      => CurrentState?.Update();
    public void FixedUpdate() => CurrentState?.FixedUpdate();

    public void OnDestroy()
    {
        CurrentState?.Exit();
        CurrentState = null;
    }
}
