using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("----- Spawners -----")]
    [SerializeField] GameObject[] spawners;
    
    [Header("----- Enemies -----")]
    // [SerializeField] GameObject enemy;
    [SerializeField] GameObject[] enemy;
    [SerializeField] int baseEnemyCount;
    [SerializeField] float enemyCountMultiplier;
    
    [Header("----- Wave -----")]
    [SerializeField] int waveIntermission;
    [SerializeField] int waveWinAmount;

    bool intermission;
    
    public int currentWave;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        spawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
        currentWave = 0;
        StartCoroutine(StartWave());
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

            if (currentWave > 1)
            {
                yield return new WaitForSeconds(waveIntermission);
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
                    Vector3 randSpawnerPoint = randSpawner.transform.position + Random.insideUnitSphere * 5; // find a point in a sphere around the spawner
                    Instantiate(enemy[Random.Range(0, enemy.Length)], randSpawnerPoint, randSpawner.transform.rotation); // Spawns a random enemy type on the random point
                }
            }
            else
            {
                for (int i = 0; i < baseEnemyCount * enemyCountMultiplier * currentWave; i++)
                {
                    GameObject randSpawner = spawners[Random.Range(0, spawners.Length)]; // Get a random spawner
                    Vector3 randSpawnerPoint = randSpawner.transform.position + Random.insideUnitSphere * 5; // find a point in a sphere around the spawner
                    Instantiate(enemy[Random.Range(0, enemy.Length)], randSpawnerPoint, randSpawner.transform.rotation); // Spawns a random enemy type on the random point
                }
            }
        }
    }
}
