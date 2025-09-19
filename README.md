# DoctorStateMachine

TL;DR: Tiny, generic, learning‑friendly state machine for Unity/.NET. Strongly‑typed actor, clean `Enter/Update/FixedUpdate/Exit`, and safe transitions (prevents re‑entrant bugs). Drop in `src/*.cs` and go.

## Why
- Minimal surface area that’s easy to grasp when learning.
- Explicit lifecycle hooks (Enter, Update, FixedUpdate, Exit).
- Generic over your actor type (`TActor`) for strong typing.
- Safe transitions: guards against re‑entrant `TransitionTo` calls.

## Install
- Unity: copy `src/*.cs` into your project (any assembly). Use the `DoctorStateMachine` namespace.
- .NET: add the three files in `src/` to your project or reference them from a shared library.

## Quick Start

Define a couple of states by subclassing `State<TActor>` and override the hooks you need.

```csharp
using DoctorStateMachine;
using System;

// Your actor type
public class Player
{
    public bool WantsToRun { get; set; }
    public float Speed { get; set; }
}

// One state
public class IdleState : State<Player>
{
    protected override void OnEnter(StateMachine<Player> sm, Player actor)
    {
        base.OnEnter(sm, actor);
        actor.Speed = 0f;
        Console.WriteLine("Entered Idle");
    }

    protected override void OnUpdate()
    {
        if (Actor.WantsToRun)
        {
            // Transition on the next frame logic path
            StateMachine.TransitionTo(new RunningState());
        }
    }
}

// Another state
public class RunningState : State<Player>
{
    protected override void OnEnter(StateMachine<Player> sm, Player actor)
    {
        base.OnEnter(sm, actor);
        actor.Speed = 5f;
        Console.WriteLine("Entered Running");
    }

    protected override void OnUpdate()
    {
        if (!Actor.WantsToRun)
        {
            StateMachine.TransitionTo(new IdleState());
        }
    }
}

// Wiring it up
var player = new Player();
var sm = new StateMachine<Player>();

sm.OnStateChanged += (prev, next) =>
{
    Console.WriteLine($"State changed: {prev?.GetType().Name ?? "<none>"} -> {next.GetType().Name}");
};

sm.Init(new IdleState(), player);

// In your game/app loop
while (true)
{
    sm.Update();      // call every frame
    // sm.FixedUpdate(); // call on a fixed timestep if you have one
}
```

Unity usage: call `sm.Update()` from `MonoBehaviour.Update()` and `sm.FixedUpdate()` from `MonoBehaviour.FixedUpdate()`. Call `sm.OnDestroy()` from `MonoBehaviour.OnDestroy()` to clean up.

## API Overview
- `StateMachine<TActor>`: manages transitions for a single actor
  - `Init(startState, actor)`: initialize once and immediately enter `startState`
  - `TransitionTo(next)`: exit current, enter next, raises `OnStateChanged`
  - `Update()` / `FixedUpdate()`: forward to current state
  - `OnDestroy()`: exit current and clear references
- `IState<TActor>`: interface for states
  - `Enter(stateMachine, actor)`, `Update()`, `FixedUpdate()`, `Exit()`
- `State<TActor>`: convenience base class
  - Gives you protected `Actor` and `StateMachine` and virtual `OnEnter/OnUpdate/OnFixedUpdate/OnExit`

## Best Practices
- Keep states focused: they should represent a single mode (e.g., Idle, Running).
- Avoid transitions in `Enter`/`Exit`: re‑entrant transitions are prevented; schedule transitions from `Update`/`FixedUpdate` instead.
- Keep long‑running work out of `Enter`/`Exit`: do only what’s needed to start/stop the mode.
- Prefer data on the actor: states coordinate behavior; the actor holds the data.
- Keep states stateless where possible: treat them like strategies with minimal internal fields.

## Design Notes
- Re‑entrancy guard: protects against bugs when a transition triggers another transition mid‑transition.
- Null‑safe forwards: `Update/FixedUpdate` are no‑ops if no state is active.
- Generic actor: you control the type—no base classes or interfaces required.

## FAQ
- Can I transition during `Enter`? Not recommended—re‑entrancy is blocked. Transition from `Update`.
- Do I need `FixedUpdate`? Only if you have a fixed timestep (e.g., Unity physics).
- Does this require Unity? No. It’s plain C# and works in any .NET app.

## Contributing
Issues and PRs are welcome. Keep the API minimal, comments clear for learners, and behavior predictable.

## License
MIT — see [LICENSE](./LICENSE).
