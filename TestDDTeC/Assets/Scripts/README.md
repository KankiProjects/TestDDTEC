# Scripts Guide

This guide summarizes the C# scripts in this folder and how they work together.

## `Player.cs` — Player movement and charged jump

Requires a `Rigidbody`. Reads movement from WASD, arrow keys, or a gamepad left stick; jump uses Space or the gamepad south button. Movement input is applied in the air relative to the camera. Hold jump while grounded to charge, then release to jump higher. The visual child squashes while charging.

Assign a visual child and, optionally, a movement camera in the Inspector. If left unassigned, the script uses the first child and the `MainCamera` camera. The player needs a collision surface to detect landing.

Inspector settings: jump force, maximum charge bonus, charge time, squash amount, air control, and maximum air speed.

## `PlayerSettings.cs` — Player settings asset

A `ScriptableObject` containing the same jump, squash, and air-control values as `Player.cs`. Create one using **Assets > Create > Player > Settings**. `Player.cs` currently stores its own values and does not read this asset.

## `EnemyStateMachine.cs` — Wandering and fleeing enemy

Requires a `Rigidbody`, which is made kinematic. The enemy starts in `Wandering`, chooses random destinations around its starting point, and moves toward them. When the player enters `fleeRadius`, it switches to `Fleeing` and moves away. Once the player leaves that radius, it returns to wandering. Movement is horizontal.

The material turns green while wandering and red while fleeing. Assign a player, or tag the player object `Player`; the enemy also needs a `Renderer` on itself or a child.

Inspector settings control the flee radius, wander area, state speeds, destination tolerance, and state colors. `CurrentState` exposes the active state.

## `Jump.cs` — Simple Space-bar jump

Requires a `Rigidbody`. Applies an upward impulse whenever Space is pressed. This is a basic standalone example; unlike `Player.cs`, it has no grounded check or charge mechanic.

## `ISmashable.cs` — Smashable interface

Defines one method, `Smash(Collision collision)`. Objects implementing it provide their own response to a collision; the collision data lets them inspect which object hit and how.

## `MovingPlatform.cs` — Moving, smashable platform

Requires a `Rigidbody`, set to kinematic at startup. Moves back and forth along the configured direction and distance. Implements `ISmashable`: it is destroyed if an object tagged `Player` lands on top.

Inspector settings: travel distance, speed, and direction. A zero direction falls back to right.

## `RespawningSmashable.cs` — Temporary smashable object

Implements `ISmashable`. When an object tagged `Player` lands on it, its child colliders and renderers are disabled, then restored after the configured delay. It ignores other collisions and prevents repeated activation while waiting to respawn.

## `PlayerTouchable.cs` — Player touch event

Raises the static `Touched` event when an object tagged `Player` enters this object's trigger or collides with it. Other scripts can subscribe to the event to respond to touches.

## `GameManager.cs` — Touch feedback

Provides a singleton `Instance` and listens for `PlayerTouchable.Touched`. If a feedback UI object is assigned, it enables it briefly after a touch; a new touch restarts the timer. The UI is hidden on startup.

Add one manager to the scene and optionally assign the feedback UI and duration.

## `Hello.cs` — Empty starter example

Contains empty `Start` and `Update` methods. It currently has no gameplay behavior and can be used as a starting point for learning Unity callbacks.

## Unity concepts used

- `Awake` initializes references and component state; `Update` handles frame-based input or checks; `FixedUpdate` handles physics movement.
- `[SerializeField]` exposes private settings in the Inspector. `[Min]` and `[Range]` help constrain their values.
- `Rigidbody`, colliders, renderers, tags, and collision callbacks connect scripts to scene objects.
- `StartCoroutine` and `WaitForSeconds` implement the delayed respawn and UI feedback.
