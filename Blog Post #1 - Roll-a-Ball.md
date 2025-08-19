# Blog Post #1: Roll-a-ball

## Introduction
In this blog post, I'll walk you through the process of creating a simple 3D game based on Unity's **Roll-a-Ball** tutorial. I'll cover the basic setup and additional features that were added to make the game more interactive and engaging. These changes include enlarging the level, adding additional enemy functions, finishing the game and adding sound effects.

## Unity's Roll-a-Ball Tutorial
Unity's **Roll-a-Ball** tutorial is a great starting point for beginners to get acquainted with Unity's basic features. The tutorial covers the following steps:

1. **Setting up the scene**: The player controls a ball that rolls around a flat surface. The objective is to collect all the items (collectibles) scattered around the environment.
2. **Player movement**: The player ball is controlled by user input to move around the scene.
3. **Enemies**: The enemies' AI is set to follow the player, and if the enemy collides with the player, the game is lost.
4. **Goal completion**: The game ends once the player collects all the items.

## Additions to the Game
After completing the initial setup from Unity’s **Roll-a-Ball** tutorial, I added several enhancements to improve the game:
1. **Expanded Level Design**: To make the game more engaging, I enlarged the level, so now it is one prolonged level. This allowed for the addition of more obstacles and enemies.
2. **Revised Goal Completion**: I redesigned the game’s objective to focus on reaching a physical finish line. This new goal is marked by a designated finish line plane. In addition, the collectibles are no longer part of the goal itself but are tracked separately as a score, allowing the player to see how much they have collected during each run.
3. **Enemy Enhancements**: To make the enemies more dynamic, I've introduced a detection radius. When the player enters this radius, the enemies begin to follow the player. Additionally, I changed the enemy speed, making it exponential. Therefore, the closer they get to the player, the faster they move
4. **Sound Additions**: To add atmosphere and immersion, I've added background music that runs when the player starts the game. I've also made it so that when a collectable is scored, a sound effect is also played.
5. **Restarting The Game**: After a player wins or loses, they can now restart the game, giving them the opportunity to try again and collect more items to increase their score.
