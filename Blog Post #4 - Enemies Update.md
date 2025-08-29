# Blog Post #4 - Enemies Update

## Introduction

This blog post covers the progress made on introducing enemies into the grappling hook game, as well as the integration of the game timer and sound feedback for player interactions with enemies.

## Progress Made

- **Enemy Implementation:**
  - Basic enemy prefabs were created with 2D sprites and aiming behavior.
  - Enemies can damage the player by reducing the remaining time on the GameTimer when touched or hit.
  - Collision detection uses 2D colliders and triggers to ensure consistent interactions.
  - Two enemies were added: Stationary rotating turret and stationary tank.
- **Timer Integration:**
  - The GameTimer now decreases when the player is hit by an enemy.
  - Timer UI is updated in real-time, showing remaining time.
  - On hitting an enemy, the timer flashes red briefly for visual feedback.
  - The timer is also incremented the moment the player uses the grappling hook. (indicated in green)
- **Audio Feedback:**
  - Each time the player loses time due to an enemy hit, a sound effect is played.
  - Enemies, background and grappling hook now have sounds.

## Challenges and Solutions

- **Accurate Collision Detection:**
  - Issue: Player sometimes passed through enemies without losing time.
  - Solution: Adjusted the colliders and ensured proper trigger settings on both enemies and the player.
- **Timer UI Feedback:**
  - Issue: Timer color change wasn’t visible in certain scenes.
  - Solution: Ensured the TimerUI script subscribes to GameTimer events on scene load and uses coroutines to handle temporary color changes.
- **Sound Timing:**
  - Issue: Multiple hits in quick succession caused overlapping or clipped sounds.
  - Solution: Implemented pitch adjustments and proper audio source handling in GameTimer.

## Next Steps for Enemies

- Add multiple enemy types with varied behaviors (patrolling, chasing, or shooting).
- Introduce visual feedback on enemies (flashing, knockback) when interacting with the player.
- Refine level design to place enemies strategically, balancing challenge with exploration.
- Test combinations of enemy hits, timer, and audio feedback to ensure smooth gameplay.