using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [SerializeField] MobSpawner[] spawners;
    [SerializeField] int waveIntermission;
    
    public int currentWave;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        StartCoroutine(StartWave());
    }
    
    public IEnumerator StartWave()
    {
        currentWave++;
        GameManager.instance.UpdateWaveCount(currentWave);
        
        if (currentWave <= spawners.Length)
        {
            yield return new WaitForSeconds(waveIntermission);
            spawners[currentWave - 1].StartWave();
        }
        else
        {
            GameManager.instance.YouWin();
        }
    }
}
