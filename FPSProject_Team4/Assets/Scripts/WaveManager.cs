using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("----- Spawners -----")]
    [SerializeField] GameObject[] spawners;
    [Range(3, 6)] [SerializeField] float spawnerRadius;
    public bool uniformRand;
    
    [Header("----- Enemies (Uniform) -----")]
    // [SerializeField] GameObject enemy;
    [SerializeField] GameObject[] enemy;
    [SerializeField] int baseEnemyCount;
    [SerializeField] float enemyCountMultiplier;

    [Header("----- Enemies (Non-Uniform) -----")]
    [SerializeField] GameObject enemyOne;
    [Range(0,1)] [SerializeField] float enemyOneProbability;
    [SerializeField] GameObject enemyTwo;
    [Range(0,1)] [SerializeField] float enemyTwoProbability;
    [SerializeField] GameObject enemyThree;
    [Range(0,1)] [SerializeField] float enemyThreeProbability;
    [SerializeField] int baseEnemyCountNU;
    [SerializeField] float enemyCountMultNU;
    
    [Header("----- Wave -----")]
    [SerializeField] int waveIntermission;
    [SerializeField] int waveWinAmount;
    public int currentWave;

    [Header("----- Coins -----")] [SerializeField]
    private int coinWaveClearBonus;

    bool intermission;

    GameObject one = null;
    GameObject two = null;
    GameObject three = null;
    int count = 0;
    float probability = 0;
    float midOne = 0;
    float midTwo = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        spawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
        currentWave = 0;
        
        if (uniformRand)
        {
            StartCoroutine(StartWave());
        }
        else
        {
            StartCoroutine(StartWaveNonUniform());
        }
    }

    public IEnumerator StartWave()
    {
        if (currentWave > waveWinAmount)
        {
            GameManager.instance.YouWin();
        }
        else
        {
            currentWave++;
            UIManager.instance.UpdateWaveCount();

            if (currentWave > 1)
            {
                yield return new WaitForSeconds(waveIntermission);
                GameManager.instance.coins += coinWaveClearBonus;
            }

            // TODO: Need to fix caculation here
            // for (int i = 0; i < baseEnemyCount * enemyCountMultiplier * currentWave; i++)
            // {
                // GameObject randSpawner = spawners[Random.Range(0, spawners.Length)];
                // Instantiate(enemy, randSpawner.transform.position, randSpawner.transform.rotation);
            // }

            if (currentWave == 0 || currentWave == 1)
            {
                for (int i = 0; i < baseEnemyCount * currentWave; i++) // First wave should always spawn base enemy count
                {
                    GameObject randSpawner = spawners[Random.Range(0, spawners.Length)]; // Get a random spawner
                    Vector3 randSpawnerPoint = randSpawner.transform.position + Random.insideUnitSphere * spawnerRadius; // find a point in a sphere around the spawner
                    Instantiate(enemy[Random.Range(0, enemy.Length)], randSpawnerPoint, randSpawner.transform.rotation); // Spawns a random enemy type on the random point
                }
            }
            else
            {
                for (int i = 0; i < baseEnemyCount * enemyCountMultiplier * currentWave; i++)
                {
                    GameObject randSpawner = spawners[Random.Range(0, spawners.Length)]; // Get a random spawner
                    Vector3 randSpawnerPoint = randSpawner.transform.position + Random.insideUnitSphere * spawnerRadius; // find a point in a sphere around the spawner
                    Instantiate(enemy[Random.Range(0, enemy.Length)], randSpawnerPoint, randSpawner.transform.rotation); // Spawns a random enemy type on the random point
                }
            }
        }
    }
    
    public IEnumerator StartWaveNonUniform()
    {
        NonUniformSetup();
        
        if (currentWave > waveWinAmount)
        {
            GameManager.instance.YouWin();
        }
        else
        {
            currentWave++;
            UIManager.instance.UpdateWaveCount();

            if (currentWave > 1)
            {
                yield return new WaitForSeconds(waveIntermission);
            }

            if (currentWave == 0 || currentWave == 1)
            {
                for (int i = 0; i < baseEnemyCountNU * currentWave; i++)
                {
                    GameObject randSpawner = spawners[Random.Range(0, spawners.Length)]; // Get a random spawner
                    Vector3 randSpawnerPoint = randSpawner.transform.position + Random.insideUnitSphere * spawnerRadius; // find a point in a sphere around the spawner
                    NonUniformEnemy(randSpawner, randSpawnerPoint);
                }
            }
            else
            {
                for (int i = 0; i < baseEnemyCountNU * enemyCountMultNU * currentWave; i++)
                {
                    GameObject randSpawner = spawners[Random.Range(0, spawners.Length)]; // Get a random spawner
                    Vector3 randSpawnerPoint = randSpawner.transform.position + Random.insideUnitSphere * spawnerRadius; // find a point in a sphere around the spawner
                    NonUniformEnemy(randSpawner, randSpawnerPoint);
                }
            }
        }
    }

    void NonUniformSetup()
    {
        if (enemyOne != null)
        {
            probability += enemyOneProbability;
            one = enemyOne;
            count++;

            if (enemyTwo != null)
            {
                midOne = probability;
                probability += enemyTwoProbability;
                two = enemyTwo;
                count++;

                if (enemyThree != null)
                {
                    midTwo = probability;
                    probability += enemyThreeProbability;
                    three = enemyThree;
                    count++;
                }
            }
            else
            {
                if (enemyThree != null)
                {
                    midOne = probability;
                    probability += enemyThreeProbability;
                    two = enemyThree;
                    count++;
                }
            }
        }
        else
        {
            if (enemyTwo != null)
            {
                probability += enemyTwoProbability;
                one = enemyTwo;
                count++;

                if (enemyThree != null)
                {
                    midOne = probability;
                    probability += enemyThreeProbability;
                    two = enemyThree;
                    count++;
                }
            }
            else
            {
                if (enemyThree != null)
                {
                    probability += enemyThreeProbability;
                    one = enemyThree;
                    count++;
                }
                else
                {
                    uniformRand = true;
                    StartCoroutine(StartWave());
                }
            }
        }
    }

    void NonUniformEnemy(GameObject spawner, Vector3 pos)
    {
        if (count == 1)
        {
            Instantiate(one, pos, spawner.transform.rotation);
        }
        else if (count == 2)
        {
            float rand = Random.Range(0, probability);

            if (rand >= 0 && rand < midOne)
            {
                Instantiate(one, pos, spawner.transform.rotation);
            }
            else
            {
                Instantiate(two, pos, spawner.transform.rotation);
            }
        }
        else if (count == 3)
        {
            float rand = Random.Range(0, probability);
            
            if (rand >= 0 && rand < midOne)
            {
                Instantiate(one, pos, spawner.transform.rotation);
            }
            else if (rand >= midOne && rand < midTwo)
            {
                Instantiate(two, pos, spawner.transform.rotation);
            }
            else
            {
                Instantiate(three, pos, spawner.transform.rotation);
            }
        }
        else
        {
            uniformRand = true;
            StartCoroutine(StartWave());
        }
    }
}
