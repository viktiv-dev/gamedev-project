# Blog Post #3 - Grappling Hook Update

## Introduction

This blog post summarizes the progress I've made, technical challenges, and new features added to the grappling hook system.

## Progress Made

- Player can aim in 2D using a joystick with an arrow indicator pointing around the player.
- Anchor Points were added, and using the aim mechanic, the player is able to select different anchor points (which current is indicated by turning the anchor point green), while making sure to always select the nearest one in case of overlapping anchor points.
- A rope is visualized using a LineRenderer and is attached to the selected anchor point.
- Once the rope is attached, the player moves to the anchor point with exponential speed (making the action feel snappy). There are two possibilities:
  - Player attaches themselves to the anchor point and remains there until further action.
  - Player cancels the action mid air to rettain some of the momentum and launch themselves further ahead.

## Challenges and Solutions

In this section I will describe some of the challenges I encountered during the process of developing the anchor point, and the solutions that were used for tackling the challenges:

- Issue: Player launching uncontrollably when canceling mid-air.
  - Solution: Reset player velocity and stop grappling forces.
- Issue: Hook always highlighted the anchor player was attached to.
  - Solution: Added ignored anchor system in AimArrow.
- Issue: Smooth pulling without the pulling motion of the player feel linear and slow.
  - Solution: Added exponential pull option and snapping when close to anchor.

## Next Steps for the Grappling Hook

- Add momentum carry-over when detaching for smoother swing feel.

- Implement audio and visual feedback for hook connecting and canceling.

- Test chaining hooks through different level layouts.