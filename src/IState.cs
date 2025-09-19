namespace DoctorStateMachine;

/// <summary>
/// Contract for a single state used by <see cref="StateMachine{TActor}"/>.
/// </summary>
/// <typeparam name="TActor">The actor type the state operates on.</typeparam>
/// <remarks>
/// States are focused objects containing the behavior for a particular mode.
/// They receive the actor and state machine on enter, update on every frame,
/// and can perform fixed-time updates if you are in a Unity-like environment.
/// </remarks>
public interface IState<TActor> where TActor : class
{
    /// <summary>
    /// Called once when this state becomes active.
    /// </summary>
    /// <param name="stateMachine">The owning state machine.</param>
    /// <param name="actor">The actor the state controls.</param>
    void Enter(StateMachine<TActor> stateMachine, TActor actor);

    /// <summary>
    /// Called every variable-timestep update (e.g., per frame).
    /// Use this for standard per-frame behavior and to call <see cref="StateMachine{TActor}.TransitionTo"/>.
    /// </summary>
    void Update();

    /// <summary>
    /// Called on a fixed timestep (e.g., Unity's FixedUpdate).
    /// Use this for physics or time-step–sensitive logic.
    /// </summary>
    void FixedUpdate();

    /// <summary>
    /// Called once just before leaving this state.
    /// </summary>
    void Exit();
}
