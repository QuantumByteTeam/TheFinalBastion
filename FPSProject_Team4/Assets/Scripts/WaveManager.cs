using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [SerializeField] GameObject[] spawners;
    [SerializeField] int waveIntermission;
    [SerializeField] int enemyCountMultiplier;
    [SerializeField] int waveWinAmount;
    
    public int currentWave;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        spawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
        StartCoroutine(StartWave());
    }
    
    public IEnumerator StartWave()
    {
        currentWave++;
        GameManager.instance.UpdateWaveCount(currentWave);
        
        if (currentWave <= spawners.Length)
        {
            yield return new WaitForSeconds(waveIntermission);
            spawners[currentWave - 1].GetComponent<MobSpawner>().StartWave();
        }
        else
        {
            GameManager.instance.YouWin();
        }
    }
}
