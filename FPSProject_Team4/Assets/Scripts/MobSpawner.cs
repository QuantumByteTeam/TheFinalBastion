using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int amountToSpawn;
    [SerializeField] float spawnIntermission;

    int amountSpawned;
    int amountKilled;
    bool isActive;
    bool activateSpawner;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.UpdateEnemyCount(amountSpawned);
    }

    // Update is called once per frame
    private void Update()
    {
        if (activateSpawner && !isActive && amountSpawned < amountToSpawn)
        {
            StartCoroutine(Spawn());
        }
    }

    public IEnumerator Spawn()
    {
        if (!isActive && amountSpawned < amountToSpawn)
        {
            isActive = true;
            
            GameObject objectSpawned = Instantiate(objectToSpawn, spawnPoints[Random.Range(0, spawnPoints.Length)].position,
                objectToSpawn.transform.rotation);
            
            if (objectSpawned.GetComponent<EnemyAI>())
            {
                objectSpawned.GetComponent<EnemyAI>().originSpawner = this;
            }
            
            amountSpawned++;
            
            yield return new WaitForSeconds(spawnIntermission);
        
            isActive = false;
        }
    }

    public void StartWave()
    {
        activateSpawner = true;
    }

    public void UpdateEnemyKilled()
    {
        amountKilled++;

        if (amountKilled <= amountToSpawn)
        {
            activateSpawner = false;
            StartCoroutine(WaveManager.instance.StartWave());
        }
    }
    
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activateSpawner = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            despawn = true;

            amountSpawned = 0;

            activateSpawner = false;
        }
    }
    */
    
}
