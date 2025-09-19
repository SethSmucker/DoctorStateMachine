using System;

namespace DoctorStateMachine;

/// <summary>
/// A small, generic state machine that manages transitions between
/// <see cref="IState{TActor}"/> instances for a single <typeparamref name="TActor"/>.
/// </summary>
/// <typeparam name="TActor">The actor type this state machine controls.</typeparam>
/// <remarks>
/// Lifecycle mirrors common game loops: call <see cref="Update"/> each frame and
/// <see cref="FixedUpdate"/> on a fixed timestep if needed. Transitions call
/// <c>Exit</c> on the previous state and <c>Enter</c> on the next.
/// Re-entrant transitions (transitioning while already transitioning) are prevented.
/// </remarks>
public class StateMachine<TActor> where TActor : class
{
    /// <summary>
    /// The actor controlled by the state machine. Set during <see cref="Init"/>.
    /// </summary>
    public TActor Actor { get; private set; }

    /// <summary>
    /// The state that is currently active, or <c>null</c> if uninitialized.
    /// </summary>
    public IState<TActor> CurrentState { get; private set; }

    /// <summary>
    /// Raised after a successful transition. Provides the previous and new states.
    /// </summary>
    public event Action<IState<TActor>, IState<TActor>> OnStateChanged;

    private bool _initialized;
    private bool _isTransitioning;

    /// <summary>
    /// Initializes the state machine with a starting state and actor.
    /// </summary>
    /// <param name="startState">The first state to enter.</param>
    /// <param name="actor">The actor the machine will control.</param>
    /// <exception cref="InvalidOperationException">Thrown if called more than once.</exception>
    public void Init(IState<TActor> startState, TActor actor)
    {
        if (_initialized) throw new InvalidOperationException("Init called more than once.");
        Actor = actor ?? throw new ArgumentNullException(nameof(actor));
        _initialized = true;
        TransitionTo(startState);
    }

    /// <summary>
    /// Transitions to the specified next state.
    /// Calls <c>Exit</c> on the current state, then <c>Enter</c> on the new state.
    /// </summary>
    /// <param name="next">The state to transition into.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="next"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// If called while a previous transition is still in progress.
    /// Avoid transitioning inside <c>Enter</c> or <c>Exit</c>—schedule for the next update instead.
    /// </exception>
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

    /// <summary>
    /// Forwards per-frame updates to the current state.
    /// Safe to call when no state is active.
    /// </summary>
    public void Update()      => CurrentState?.Update();

    /// <summary>
    /// Forwards fixed-timestep updates to the current state.
    /// Safe to call when no state is active.
    /// </summary>
    public void FixedUpdate() => CurrentState?.FixedUpdate();

    /// <summary>
    /// Cleans up the current state by calling <c>Exit</c> and clearing references.
    /// Call this from your host's teardown (e.g., Unity's <c>OnDestroy</c>).
    /// </summary>
    public void OnDestroy()
    {
        CurrentState?.Exit();
        CurrentState = null;
    }
}
