using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [System.Serializable]
    public class IntermissionTimes
    {
        [Tooltip("The intermission time at the very start of a new game.")] public float start;
        [Tooltip("The regular intermission time in seconds.")] public float regular;
        [Tooltip("The special round intermission time in seconds. This time will be used after a special round is completed.")] public float afterSpecial;
        [Tooltip("The boss round intermission time in seconds. This time will be used after a boss round is completed.")] public float afterBoss;
    }

    [System.Serializable]
    public class RoundData
    {
        [System.Serializable]
        public class EnemyDataInternal
        {
            [Tooltip("The enemy to use.")] public EnemyData probabilityData;
            [Tooltip("The probability this enemy will spawn during this wave."), Range(0, 1)] public float probability;
        }

        [Tooltip("The name of this wave.")] public string name;
        [Tooltip("The probability this wave will happen."), Range(0, 1)] public float probability;

        [Tooltip("The data for each enemy. Ensure that the total is equal to 1")] public List<EnemyDataInternal> enemyData;
    }

    [Tooltip("The current wave of the game."), SerializeField] public int currentWave;
    [Tooltip("The value at which to multiply by to get the next amount of enemies."), Range(1, 100), SerializeField] float enemyCountMultiplier = 1;
    [Tooltip("The interval of when boss round will appear. Example: this set to 5, rounds 5, 10, 15, 20, etc will be boss rounds"), Range(1, 1000)] public int bossRoundInterval = 2;
    [Tooltip("All the enemy spawners in the scene. Automatically populates."), HideInInspector] public List<EnemySpawner> spawners;
    [Tooltip("All the round data. Regular round data is built-in."), SerializeField] List<RoundData> roundData = new List<RoundData> { new RoundData { name = "regular", probability = 1 } };

    [Tooltip("The times for each intermission type."), SerializeField] IntermissionTimes intermissionTimes;

    private bool isRunning = false;

    void OnValidate()
    {
        bossRoundInterval = Math.Max(0, bossRoundInterval);
        intermissionTimes.start = Mathf.Max(0, intermissionTimes.start);
        intermissionTimes.regular = Mathf.Max(0, intermissionTimes.regular);
        intermissionTimes.afterSpecial = Mathf.Max(0, intermissionTimes.afterSpecial);
        intermissionTimes.afterBoss = Mathf.Max(0, intermissionTimes.afterBoss);

        RoundData foundRound = roundData.Find(data => data.name == "regular");

        if (foundRound != null)
        {
            float totalRoundProb = 0;

            foreach (RoundData data in roundData)
            {
                if (data != foundRound)
                {
                    totalRoundProb += data.probability;
                }
            }

            if (totalRoundProb > 1)
            {
                Debug.LogError("RoundData total probability is greater than 1. Total enemyData probability = " + totalRoundProb);
            }
            else
            {
                foundRound.probability = 1 - totalRoundProb;
            }
        }
        else
        {
            Debug.LogError("No round 'regular' found in WaveManager.roundData");
        }
    }

    void Awake()
    {
        instance = this;
        currentWave = 0;
    }

    void Start()
    {
        spawners.AddRange(GameObject.FindGameObjectsWithTag("EnemySpawner"));
    }

    public void Run()
    {
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    private IEnumerator StartWave()
    {
        currentWave++;
        UIManager.instance.UpdateWaveCount();

        if (currentWave > 1)
        {
            yield return new WaitForSeconds(intermissionTimes.regular);
            UIManager.instance.UpdateBalance();
        }
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{
    SerializedProperty enemyCountMultiplier;
    SerializedProperty bossInterval;
    SerializedProperty roundData;
    SerializedProperty intermissionTimes;

    private void OnEnable()
    {
        enemyCountMultiplier = serializedObject.FindProperty("enemyCountMultiplier");
        bossInterval = serializedObject.FindProperty("bossRoundInterval");
        roundData = serializedObject.FindProperty("roundData");
        intermissionTimes = serializedObject.FindProperty("intermissionTimes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(enemyCountMultiplier);
        EditorGUILayout.PropertyField(bossInterval);
        EditorGUILayout.PropertyField(roundData);
        EditorGUILayout.PropertyField(intermissionTimes);
        EditorGUILayout.HelpBox("Ensure that the total of enemyData probabilities in each roundData is equal to 1", MessageType.Warning);

        serializedObject.ApplyModifiedProperties();
    }
}