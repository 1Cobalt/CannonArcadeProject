# CannonArcadeProject
A dynamic 2D arcade shooter where players defend against waves of falling and path-following obstacles. The goal is to survive, collect coins, upgrade the cannon's arsenal, and defeat massive bosses to unlock new stages.

Gameplay & Features

- Dual Spawner System: Face enemies that drop vertically with randomized physics or Zuma-style waypoints.
- Boss Encounters: Survive intense battles against massive bosses.
- Deep Upgrade Shop: Spend collected coins to buy permanent upgrades
- Level Progression: Unlock levels sequentially. Enemy health scales dynamically based on the current stage, encouraging players to grind earlier levels to meet DPS checks.

Technical Highlights

- Game Balancing & Math: Applied a logarithmic formula (Mathf.Log) to the Fire Rate upgrade to implement diminishing returns, preventing late-game balancing issues.
- Dynamic Difficulty Scaling: Automated enemy HP calculation depending on level.
- Architecture & Audio: Utilized the Singleton pattern for robust GameManager and AudioManager implementations/
- Dynamic UI: Built a scalable, scrolling Level Selection menu using Unity's UGUI (Scroll View, Grid Layout Group) that automatically updates lock/unlock states via PlayerPrefs.
