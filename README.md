# Strategy + Flyweight + EFSM Tower Defense Practice

A Unity tower defense prototype that showcases **Strategy**, **Flyweight**, and **Extended Finite State Machine (EFSM)** patterns working together. The project focuses on modular tower/enemy behavior, reusable data objects, and event-driven gameplay updates.

## What this project is for

Use this repository as a learning/reference project for:

- **Strategy pattern**: swap AI behaviors via `ScriptableObject` strategies.
- **Flyweight pattern**: cache tower data to avoid redundant asset loading.
- **EFSM**: drive entity behavior transitions with explicit state definitions.
- **Event-driven UI/gameplay**: broadcast card and hand updates through a central event manager.

## Requirements

- **Unity 2022.3.47f1** (see `ProjectSettings/ProjectVersion.txt`).
- Unity packages resolved via the `Packages/manifest.json` file.

## How to run

1. Open the project in **Unity 2022.3.47f1**.
2. Load the main scene: `Assets/Scenes/Gameplay.unity`.
3. Press **Play** to run the game inside the Unity editor.

### Build (optional)

1. Open **File → Build Settings**.
2. Ensure `Gameplay` is included in the build scenes list.
3. Select your target platform and click **Build**.

## Project structure (high level)

- `Assets/Assets/Scripts/Gameplay/Core/EFSM/` — EFSM implementation (`State`, `EventFSM`, `StateMachine`).
- `Assets/Assets/Scripts/Gameplay/Core/Strategy/` — Strategy abstractions and concrete strategies.
- `Assets/Assets/Scripts/Gameplay/Core/Tower/` — Tower entity and Flyweight data/loader.
- `Assets/Assets/Scripts/Gameplay/Core/Enemy/` — Enemy entity logic.
- `Assets/Assets/Scripts/Gameplay/Core/Card/` — Card database and UI-facing card data.
- `Assets/Assets/Scripts/Manager/` — Event, grid, pool, and upgrade managers.
- `Assets/Assets/Resources/LocalDB/Cards.json` — Local card data loaded at runtime.
- `Assets/Assets/ScriptableObjects/` — Strategy assets referenced by ID.

## How the game works

### 1) EFSM for entity behavior
`Tower` and `Enemy` entities inherit from `StateMachine<T>` and define their state transitions. Each state dispatches to a strategy implementation to execute logic.

- Towers: `Idle` ⇄ `Attack`.
- Enemies: `Walking` ⇄ `Attack`.

### 2) Strategy pattern for actions
Strategies are `ScriptableObject` assets that encapsulate behavior (e.g., attack, idle, walking). They’re injected into entities at runtime so the same entity can behave differently without code changes.

### 3) Flyweight for tower data
The tower’s static data is stored in a `TowerFlyweight` object created via `TowerFlyweightFactory`. Flyweights are cached in memory and reused by towers, avoiding redundant asset loads.

### 4) Card data pipeline
`CardsDatabase` loads `Assets/Assets/Resources/LocalDB/Cards.json`, then preloads flyweights and triggers `EventName.CardsLoaded` once data is ready. UI and gameplay systems subscribe through the `EventManager`.

## Runtime API (internal C# API)

This project does **not** expose an HTTP/REST API. Instead, it provides an internal C# API you can use from gameplay and UI code.

### Event system
Use `EventManager` to subscribe to and broadcast gameplay events.

```csharp
EventManager.Instance.Subscribe(EventName.CardsLoaded, args => { /* ... */ });
EventManager.Instance.Trigger(EventName.HandUpdate, payload);
```

### EFSM helpers
Use `StateMachine<T>`, `State`, and `EventFSM<T>` to define state-driven behavior.

```csharp
public class Tower : StateMachine<TowerStates>
{
    public override void ConfigureFSM()
    {
        var idle = new State<TowerStates>("IdleState");
        var attacking = new State<TowerStates>("AttackState");
        StateConfigurer.Create(idle).SetTransition(TowerStates.Attack, attacking).Done();
        StateConfigurer.Create(attacking).SetTransition(TowerStates.Idle, idle).Done();
        _fsm = new EventFSM<TowerStates>(idle);
    }
}
```

### Strategy contracts
Create new behaviors by implementing the `Strategy<T>` base class and creating ScriptableObject assets.

```csharp
public class ArrowAttackStrategy : Strategy<Tower>
{
    public override void OnUpdate(Tower tower)
    {
        // attack logic
    }
}
```

### Flyweight access
Request or preload flyweights through the factory.

```csharp
TowerFlyweight flyweight = await TowerFlyweightFactory.GetFlyweight(card);
await TowerFlyweightFactory.PreloadFlyweights(cards);
```

## Configuration and data

- **Card data**: update `Assets/Assets/Resources/LocalDB/Cards.json` to add or adjust tower stats.
- **Strategy assets**: add new `Attack_*.asset` or `Idle_*.asset` in `Assets/Assets/ScriptableObjects/`.
- **Sprites**: tower/projectile/icon sprites are loaded via Addressables in `TowerFlyweightFactory`.

## Notes

- This is a Unity editor project; no dedicated server or backend is required.
- Addressable asset paths are hard-coded in the flyweight factory, so sprite names must match the JSON data.
