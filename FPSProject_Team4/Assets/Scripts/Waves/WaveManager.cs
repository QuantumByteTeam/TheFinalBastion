using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [System.Serializable]
    public class IntermissionTimes
    {
        [Header("All values are in seconds")]
        [Tooltip("The intermission time at the very start of a new game.")] public float start;
        [Tooltip("The regular intermission time in seconds.")] public float afterRegular;
        [Tooltip("The special round intermission time in seconds. This time will be used after a special round is completed.")] public float afterSpecial;
        [Tooltip("The boss round intermission time in seconds. This time will be used after a boss round is completed.")] public float afterBoss;
    }

    [System.Serializable]
    public class RoundData
    {
        [System.Serializable]
        public class EnemyDataInternal
        {
            [Tooltip("The enemy to use.")] public EnemyData enemyRef;
            [Tooltip("The probability this enemy will spawn during this wave."), Range(0, 1)] public float probability;
        }

        [Tooltip("The name of this wave.")] public string name;
        [Tooltip("The type of wave this is.")] public WaveType type;
        [Tooltip("The probability this wave will happen."), Range(0, 1)] public float probability;

        [Tooltip("The data for each enemy. Ensure that the total is equal to 1")] public List<EnemyDataInternal> enemyData;
    }

    [Tooltip("The current wave of the game."), SerializeField] int currentWave;
    [SerializeField] int startEnemyCount = 1;
    [Tooltip("The value at which to multiply by to get the next amount of enemies."), SerializeField] float enemyCountMultiplier = 1;
    [Tooltip("The interval of when boss round will appear. Example: this set to 5, rounds 5, 10, 15, 20, etc will be boss rounds"), HideInInspector] public int bossRoundInterval = 2;
    [Tooltip("All the enemy spawners in the scene. Automatically populates."), HideInInspector] public List<EnemySpawner> spawners;
    [Tooltip("The spawn delay between each enemy spawning.")] public float spawnDelay = 0;
    [Tooltip("All the round data. Regular round data is built-in."), SerializeField] List<RoundData> roundData = new List<RoundData> { new RoundData { name = "regular", probability = 1 } };

    [Tooltip("The times for each intermission type."), SerializeField] IntermissionTimes intermissionTimes;

    private bool isRunning = false;
    private bool isIntermissionActive = false;
    private bool isWaveRunning = false;
    private RoundData round;
    private int totalEnemyCount;

    void OnValidate()
    {
        startEnemyCount = Math.Max(1, startEnemyCount);
        enemyCountMultiplier = Mathf.Max(1, enemyCountMultiplier);
        bossRoundInterval = Math.Max(1, bossRoundInterval);

        intermissionTimes.start = Mathf.Max(0, intermissionTimes.start);
        intermissionTimes.afterRegular = Mathf.Max(0, intermissionTimes.afterRegular);
        intermissionTimes.afterSpecial = Mathf.Max(0, intermissionTimes.afterSpecial);
        intermissionTimes.afterBoss = Mathf.Max(0, intermissionTimes.afterBoss);

        ValidateRoundData();
    }

    void Awake()
    {
        ValidateRoundData(true);

        instance = this;
        currentWave = 0;
    }

    void Start()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EnemySpawner"))
        {
            EnemySpawner spawner = obj.GetComponent<EnemySpawner>();

            if (spawner != null)
            {
                spawners.Add(spawner);
            }
            else
            {
                Debug.LogError("GameObject found with tag 'EnemySpawner' but does not have component 'EnemySpawner'.");
            }
        }
    }

    public void OnWaveEnd()
    {
        isWaveRunning = false;

        float time;
        switch (round.type)
        {
            case WaveType.special:
                time = intermissionTimes.afterSpecial;
                break;
            case WaveType.boss:
                time = intermissionTimes.afterBoss;
                break;
            default:
                time = intermissionTimes.afterRegular;
                break;
        }

        StartCoroutine(StartIntermission(time));
    }

    public void Run()
    {
        isRunning = true;
        StartCoroutine(StartIntermission(intermissionTimes.start));
    }

    IEnumerator StartIntermission(float intermissionTime)
    {
        isIntermissionActive = true;
        yield return new WaitForSeconds(intermissionTime);
        isIntermissionActive = false;

        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        isWaveRunning = true;
        currentWave++;
        UIManager.instance.UpdateWaveCount();

        int enemyCount = Mathf.FloorToInt(startEnemyCount + enemyCountMultiplier * currentWave); // Enemy Count function, currently is linear
        totalEnemyCount = enemyCount;

        round = GetRandomRound();

        for (int i = 0; i < enemyCount; i++)
        {
            RoundData.EnemyDataInternal enemy = GetRandomEnemy();
            EnemySpawner spawner = spawners[UnityEngine.Random.Range(0, spawners.Count)];
            spawner.TriggerSpawner(enemy.enemyRef.enemy);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    RoundData GetRandomRound()
    {
        List<RoundData> rounds = roundData.FindAll(round => round.probability > 0);

        List<float> probabilities = new List<float>();

        float totalRoundProb = 0;

        foreach (RoundData data in roundData)
        {
            totalRoundProb += data.probability;
            probabilities.Add(totalRoundProb);
        }

        float random = UnityEngine.Random.Range(0f, totalRoundProb);

        int roundIndex = probabilities.FindIndex(prob => prob >= random);

        return rounds[roundIndex];
    }

    RoundData.EnemyDataInternal GetRandomEnemy()
    {
        List<RoundData.EnemyDataInternal> enemies = round.enemyData.FindAll(enemy => enemy.probability > 0);

        List<float> probabilities = new List<float>();

        float totalRoundProb = 0;

        foreach (RoundData.EnemyDataInternal data in round.enemyData)
        {
            totalRoundProb += data.probability;
            probabilities.Add(totalRoundProb);
        }

        float random = UnityEngine.Random.Range(0f, totalRoundProb);

        int enemyIndex = probabilities.FindIndex(prob => prob >= random);

        return enemies[enemyIndex];
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public RoundData GetActiveRound()
    {
        return round;
    }

    public bool GetIsRunning()
    {
        return isRunning;
    }

    public bool GetIsIntermissionActive()
    {
        return isIntermissionActive;
    }

    public bool GetIsWaveActive()
    {
        return isWaveRunning;
    }

    public int GetTotalEnemies()
    {
        return totalEnemyCount;
    }

    void ValidateRoundData(bool debug = false)
    {
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
                else
                {
                    data.type = WaveType.regular;
                }

                float totalEnemyProb = 0;
                foreach (RoundData.EnemyDataInternal enemyData in data.enemyData)
                {
                    totalEnemyProb += enemyData.probability;
                }

                if ((totalEnemyProb > 1 || totalEnemyProb < 1) && debug)
                {
                    Debug.LogError("EnemyData total probability is greater/less than 1. Total enemyData probability = " + totalEnemyProb);
                }
            }

            if (totalRoundProb > 1 && debug)
            {
                Debug.LogError("RoundData total probability is greater than 1. Total roundData probability = " + totalRoundProb);
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
}

public enum WaveType
{
    regular,
    special,
    boss
}

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{
    SerializedProperty startEnemyCount;
    SerializedProperty enemyCountMultiplier;
    SerializedProperty bossInterval;
    SerializedProperty spawnDelay;
    SerializedProperty roundData;
    SerializedProperty intermissionTimes;

    private void OnEnable()
    {
        startEnemyCount = serializedObject.FindProperty("startEnemyCount");
        enemyCountMultiplier = serializedObject.FindProperty("enemyCountMultiplier");
        bossInterval = serializedObject.FindProperty("bossRoundInterval");
        spawnDelay = serializedObject.FindProperty("spawnDelay");
        roundData = serializedObject.FindProperty("roundData");
        intermissionTimes = serializedObject.FindProperty("intermissionTimes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(startEnemyCount);
        EditorGUILayout.PropertyField(enemyCountMultiplier);
        EditorGUILayout.PropertyField(bossInterval);
        EditorGUILayout.PropertyField(spawnDelay);
        EditorGUILayout.PropertyField(roundData);
        EditorGUILayout.PropertyField(intermissionTimes);
        EditorGUILayout.HelpBox("Ensure that the total of enemyData probabilities in each roundData is equal to 1", MessageType.Warning);

        serializedObject.ApplyModifiedProperties();
    }
}