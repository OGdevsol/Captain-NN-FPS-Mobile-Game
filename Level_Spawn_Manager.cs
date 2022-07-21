using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Spawn_Manager : MonoBehaviour
{
	public static Level_Spawn_Manager instance;
	public GameObject[] enemiesVariantsPrefabs;
	public Level[] level;
	private int x;
	public TMP_Text waveObjectiveGameObject;


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
		AddWavesToList();
		InitializeEnemies();
	}

	private void
		AddWavesToList() // Method to initialize enemies in form of waves. When all enemies in the first wave are killed in a level, the second wave will be initiated. Level in completed when all waves are killed
	{
		var wavesLengthInLevel = level[0].waves.Length;
		int i;
		for (i = 0; i < wavesLengthInLevel; i++)
		{
			_currentWaveInLoop = i;
			level[0].wavesInLevel.Add(level[0].waves[i]);
		}
	}

	private void InitializeEnemies()
	{
		waveObjectiveGameObject.text = level[0].wavesInLevel[_currentWaveToKeepActiveIndex].waveObjective;
		for (int j = 0; j < level[0].wavesInLevel[_currentWaveToKeepActiveIndex].enemyType.Length; j++)
		{
			var E = Instantiate(enemiesVariantsPrefabs[CheckEnemiesType(j)],
				level[0].wavesInLevel[_currentWaveToKeepActiveIndex].enemyPosition[j].position,
				level[0].waves[_currentWaveToKeepActiveIndex].enemyPosition[j].rotation);
			level[0].wavesInLevel[_currentWaveToKeepActiveIndex].enemiesGameObjectInWave.Add(E.transform);
		}
	}


	private static int waveToBeRemovedIndex = 0;

	public void
		CheckEnemiesInActiveWave() // When there are zero enemies left in currently active wave, next enemy wave will be initialized (Also being used in health script of the AI in AI Kit in KillAI method. NOTE: Don't forget to remove the enemy gameobject from respective wave when it is killed 
	{
		if (level[0].wavesInLevel[0].enemiesGameObjectInWave.Count == 0)
		{
			level[0].wavesInLevel
				.RemoveAt(waveToBeRemovedIndex); // Wave with zero enemies will be removed/deactivated to initialize next wave and the next wave will take its place at waveToBeRemovedIndex. The cycle will repeat when 0 enemies are left in new wave.

			if (level[0].wavesInLevel.Count > 0)
			{
				InitializeEnemies();
			}
		}

		if (level[0].wavesInLevel.Count == 0)
		{
			Debug.Log("LEVEL COMPLETED");
			waveObjectiveGameObject.text = "Level Completed";
		}
	}


	private int
		CheckEnemiesType(int i) // Method to check which enemy type is selected in the editor in a level wave so THAT particular enemy can be instantiated in its respective position using InitializeEnemies() method
	{
		switch (level[0].wavesInLevel[_currentWaveToKeepActiveIndex].enemyType[i])
		{
			case EnemyType.AlienSoldier:
				x = 0;
				return x;
			case EnemyType.TurnedSoldier:
				x = 1;
				return x;
			case EnemyType.Mutant:
				x = 2;
				return x;
			case EnemyType.AlienPlant:
				x = 3;
				return x;
			case EnemyType.AlienBoss:
				x = 4;
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
		Mutant,
		AlienPlant,
		AlienBoss
	}
}

[Serializable]
public class Wave
{
	public string waveObjective;
	public Level_Spawn_Manager.EnemyType[] enemyType;
	public Transform[] enemyPosition;

	[HideInInspector] public List<Transform>
		enemiesGameObjectInWave; // Each level's waves' enemies gameobjects will be placed in this list according to their waves placement. 
}

[Serializable]
public class Level
{
	public Wave[] waves; // Add Enemies Details in this array


	[HideInInspector] public List<Wave>
		wavesInLevel; //Total waves in each level will be added to this list to add and maintain functionality control over each wave's properties
}