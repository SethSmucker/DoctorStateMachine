namespace DefaultNamespace;

public interface IState<TActor> where TActor : class
{
    void Enter(StateMachine<TActor> stateMachine, TActor actor);
    void Update();
    void FixedUpdate();
    void Exit();
}