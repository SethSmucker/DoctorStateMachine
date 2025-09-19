using System;

namespace DoctorStateMachine;

/// <summary>
/// Convenience base class for building states.
/// Provides strongly-typed <see cref="Actor"/> and <see cref="StateMachine"/>
/// references and no-op virtual hooks to override.
/// </summary>
/// <typeparam name="TActor">The actor type this state controls.</typeparam>
public abstract class State<TActor> : IState<TActor> where TActor : class
{
    /// <summary>
    /// The owning state machine, set in <see cref="OnEnter(StateMachine{TActor}, TActor)"/>.
    /// </summary>
    protected StateMachine<TActor> StateMachine { get; private set; }

    /// <summary>
    /// The actor this state controls, set in <see cref="OnEnter(StateMachine{TActor}, TActor)"/>.
    /// </summary>
    protected TActor Actor { get; private set; }

    /// <inheritdoc />
    public void Enter(StateMachine<TActor> stateMachine, TActor actor) => OnEnter(stateMachine, actor);

    /// <inheritdoc />
    public void Update()      => OnUpdate();

    /// <inheritdoc />
    public void FixedUpdate() => OnFixedUpdate();

    /// <inheritdoc />
    public void Exit()        => OnExit();

    /// <summary>
    /// Called once when the state becomes active. Stores references to the owning
    /// <paramref name="stateMachine"/> and <paramref name="actor"/> for use during the state.
    /// </summary>
    /// <param name="stateMachine">The owning state machine.</param>
    /// <param name="actor">The actor the state will operate on.</param>
    protected virtual void OnEnter(StateMachine<TActor> stateMachine, TActor actor)
    {
        StateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        Actor = actor ?? throw new ArgumentNullException(nameof(actor));
    }

    /// <summary>
    /// Per-frame update hook for variable timestep logic.
    /// </summary>
    protected virtual void OnUpdate() { }

    /// <summary>
    /// Fixed timestep update hook (e.g., physics logic).
    /// </summary>
    protected virtual void OnFixedUpdate() { }

    /// <summary>
    /// Called once just before leaving the state.
    /// </summary>
    protected virtual void OnExit() { }
}
