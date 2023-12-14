using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("----- Spawners -----")]
    [SerializeField] GameObject[] spawners;
    [Range(3, 6)] [SerializeField] float spawnerRadius;
    public bool uniformRand;

    [Header("----- Enemies -----")]
    [SerializeField] EnemyProbability[] enemyP;
    

    [Header("----- Enemies (Uniform) -----")]
    [SerializeField] int baseEnemyCount;
    [SerializeField] float enemyCountMultiplier;

    [Header("----- Enemies (Non-Uniform) -----")]
    [SerializeField] int baseEnemyCountNU;
    [SerializeField] float enemyCountMultNU;
    
    [Header("----- Wave -----")]
    [SerializeField] int waveIntermission;
    [SerializeField] int waveWinAmount;
    public int currentWave;

    [Header("----- Coins -----")] [SerializeField]
    private int coinWaveClearBonus;

    bool intermission;

    int length;
    
    float probability = 0;
    List<float> midNums = new List<float>();
    
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
                    Instantiate(enemyP[Random.Range(0, enemyP.Length)].enemy, randSpawnerPoint, randSpawner.transform.rotation);
                }
            }
            else
            {
                for (int i = 0; i < baseEnemyCount * enemyCountMultiplier * currentWave; i++)
                {
                    GameObject randSpawner = spawners[Random.Range(0, spawners.Length)]; // Get a random spawner
                    Vector3 randSpawnerPoint = randSpawner.transform.position + Random.insideUnitSphere * spawnerRadius; // find a point in a sphere around the spawner
                    Instantiate(enemyP[Random.Range(0, enemyP.Length)].enemy, randSpawnerPoint, randSpawner.transform.rotation);
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
        midNums.Add(0);

        for (int i = 0; i < enemyP.Length; i++)
        {
            probability += enemyP[i].probability;
            midNums.Add(probability);
        }
    }

    void NonUniformEnemy(GameObject spawner, Vector3 pos)
    {
        if (enemyP.Length == 1)
        {
            Instantiate(enemyP[0].enemy, pos, spawner.transform.rotation);
        }
        else if (enemyP.Length > 1)
        {
            float rand = Random.Range(0, probability);

            for (int i = 0; i < enemyP.Length; i++)
            {
                if (rand >= midNums[i] && rand < midNums[i + 1])
                {
                    Instantiate(enemyP[i].enemy, pos, spawner.transform.rotation);
                }
            }
        }
        else
        {
            uniformRand = true;
            StartCoroutine(StartWave());
        }
    }
}
