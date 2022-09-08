# Levels_EnemiesSpawner
This is a simple and efficient wave spawner for a game with multiple enemy types. All you have to include in addition is removing the enemy from the list
being created on runtime of levels waves. Remove the enemy that has just been killed from it (Mostly found where enemy health <=  0 enemy health script) and 
when all enemies are removed from the duplicated list, the next wave is initiated and when there are no more waves to initialize, the level is completed.
