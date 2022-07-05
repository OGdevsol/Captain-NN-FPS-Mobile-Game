using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Spawn_Manager : MonoBehaviour
{
	public static Level_Spawn_Manager instance;
	public GameObject[] enemiesVariantsPrefabs;
	public Level[] level;
	private int x;

	private int
		_currentWaveInLoop; // To only check and instantiate according to the total enemies kept in each wave while initializing enemy instantiation

	[HideInInspector] public int
		_currentWaveToKeepActiveIndex; // To activate a single wave at a given time. Next wave will be activated when all enemies of currently active wave are killed

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		InitializeEnemies(); // All enemies will be instantiated and their game objects + transforms will be assigned to their respective waves
		EnableOnlyCurrentWave(); // All enemies will be deactivated and only those which are in "Current Wave" will be kept active
	}

	private void
		InitializeEnemies() // Method to initialize enemies in form of waves.  // All enemies will be instantiated and their game objects + transforms will be assigned to their respective waves
	{
		var wavesLengthInLevel = level[0].waves.Length;
		int i;
		for (i = 0; i < wavesLengthInLevel; i++)
		{
			_currentWaveInLoop = i;
			level[0].wavesInLevel.Add(level[0].waves[i]);
			Debug.Log("Instantiating Wave " + _currentWaveInLoop);
			for (int j = 0; j < level[0].wavesInLevel[_currentWaveInLoop].enemyType.Length; j++)
			{
				var E = Instantiate(enemiesVariantsPrefabs[CheckEnemiesType(j)],
					level[0].waves[_currentWaveInLoop].enemyPosition[j].position,
					level[0].waves[_currentWaveInLoop].enemyPosition[j].rotation);
				level[0].totalEnemies.Add(E.transform);
				level[0].waves[_currentWaveInLoop].enemiesGameObjectInWave.Add(E.gameObject);
			}
		}
	}

	private void
		EnableOnlyCurrentWave() // A method to check which wave is currently active and how many enemies are left in said wave before initializing next wave. All enemies will be deactivated and only those which are in "Current Wave" will be kept active
	{
		_currentWaveToKeepActiveIndex = 0;
		int i;
		int j;
		int wavesInLevelCount = level[0].wavesInLevel.Count;

		for (i = 0; i < wavesInLevelCount; i++)
		{
			for (j = 0; j < level[0].wavesInLevel[i].enemyType.Length; j++)
			{
				level[0].wavesInLevel[i].enemiesGameObjectInWave[j].SetActive(false);
				level[0].wavesInLevel[_currentWaveToKeepActiveIndex].enemiesGameObjectInWave[j].SetActive(true);
			}
		}
	}

	private static int waveToBeRemovedIndex = 0;

	public void
		CheckEnemiesInActiveWave() // When there are zero enemies left in currently active wave, next enemy wave will be initialized (Also being used in health script of the AI in AI Kit in KillAI method. NOTE: Don't forget to remove the enemy gameobject from respective wave when it is killed 
	{
		if (level[0].wavesInLevel[_currentWaveToKeepActiveIndex].enemiesGameObjectInWave.Count == 0)
		{
			level[0].wavesInLevel
				.RemoveAt(waveToBeRemovedIndex); // Wave with zero enemies will be removed/deactivated to initialize next wave and the next wave will take its place at waveToBeRemovedIndex. The cycle will repeat when 0 enemies are left in new wave.
			EnableOnlyCurrentWave();
		}

		if (level[0].wavesInLevel.Count == 0)
		{
			Debug.Log("LEVEL COMPLETED");
		}
	}


	private int
		CheckEnemiesType(int i) // Method to check which enemy type is selected in the editor in a level wave so THAT particular enemy can be instantiated in its respective position using InitializeEnemies() method
	{
		switch (level[0].waves[_currentWaveInLoop].enemyType[i])
		{
			case EnemyType.AlienSoldier:
				x = 0;
				return x;
			case EnemyType.TurnedSoldier:
				x = 1;
				return x;
			case EnemyType.AlienPlant:
				x = 2;
				return x;
			case EnemyType.AlienBoss:
				x = 3;
				return x;
			default:
				return x;
		}
	}

	[Serializable]
	public enum EnemyType
	{
		AlienSoldier,
		TurnedSoldier,
		AlienPlant,
		AlienBoss
	}
}

[Serializable]
public class Wave
{
	public Level_Spawn_Manager.EnemyType[] enemyType;
	public Transform[] enemyPosition;

	[HideInInspector] public List<GameObject>
		enemiesGameObjectInWave; // Each level's waves' enemies gameobjects will be placed in this list according to their waves placement. 
}

[Serializable]
public class Level
{
	public Wave[] waves; // Add Enemies Details in this array

	[HideInInspector] public List<Transform>
		totalEnemies; //Total enemies in each level will be added to this list to easily manage their in-game/level functionality

	[HideInInspector] public List<Wave>
		wavesInLevel; //Total waves in each level will be added to this list to add and maintain functionality control over each wave's properties
}