using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [SerializeField] GameObject[] spawners;
    [SerializeField] int baseEnemyCount;
    [SerializeField] int waveIntermission;
    [SerializeField] float enemyCountMultiplier;
    [SerializeField] int waveWinAmount;
    [SerializeField] GameObject enemy;

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
            for (int i = 0; i < baseEnemyCount * enemyCountMultiplier * currentWave; i++)
            {
                GameObject randSpawner = spawners[Random.Range(0, spawners.Length)];
                Instantiate(enemy, randSpawner.transform.position, randSpawner.transform.rotation);
            }
        }
    }
}
