using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Linq;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public GameObject playerSpawnPos;
    public PlayerController playerScript;

    public GameObject point;
    public PointController pointScript;

    public GameObject wave;
    public WaveManager waveScript;
    
    public bool isPaused;
    public bool isActivePaused;
    public int enemiesRemaining;
    // public int waveCount;
    public int coins;
    float timescaleOG;

    public int score;
    public Store accessedStore;

    public UnityEvent waveEndEvent;
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPos = GameObject.FindWithTag("PlayerSpawn");
        point = GameObject.FindWithTag("Point");
        //pointScript = point.GetComponent<PointController>();
        wave = GameObject.FindWithTag("WaveManager");
        waveScript = wave.GetComponent<WaveManager>();
        timescaleOG = Time.timeScale;
        coins = 0;

    }

    void Start()
    {
        WaveManager.instance.Run();
    }

    // Update is called once per frame
    void Update()
    {
        if (UIManager.instance.menuActive == null)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                StatePaused();
                UIManager.instance.DisplayPausedMenu();
            }
        }
        UIManager.instance.UpdatePointIndicators();
    }

    public void StatePaused()
    {
        isPaused = !isPaused;
        playerScript.aud.Pause();
        AudioListener.pause = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpaused()
    {
        isPaused = !isPaused;
        Time.timeScale = timescaleOG;
        playerScript.aud.UnPause();
        AudioListener.pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        UIManager.instance.CloseActiveMenu();
    }

    public void ActivePaused()
    {
        isActivePaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ActiveUnpause()
    {
        isActivePaused = !isActivePaused;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateEnemyCount(int amount)
    {
        enemiesRemaining += amount;
        enemiesRemaining = Mathf.Max(enemiesRemaining, 0);
        UIManager.instance.UpdateRemainingEnemies();

        if (enemiesRemaining <= 0)
        {
            waveEndEvent.Invoke();
        }
    }
    
    public void resetGuns()
    {
        for (int i = 0; i < playerScript.inventory.hotbarInventory.Count; i++)
        {
            if (playerScript.inventory.hotbarInventory.ElementAt(i).Key.isGun)
            {
                playerScript.inventory.hotbarInventory.ElementAt(i).Key.firstInstance = true;
            }
        }

        GameObject[] guns = GameObject.FindGameObjectsWithTag("Gun");

        foreach (GameObject gun in guns)
        {
            gun.GetComponent<GunPickup>().gun.firstInstance = true;
        }
        
    }

    // public void UpdateWaveCount(int amount)
    // {
    //     waveCount += amount;
    //     UIManager.instance.UpdateWaveCount();
    // }

    public void YouWin()
    {
        StatePaused();
        UIManager.instance.DisplayWinMenu();
    }

    public void YouLose()
    {
        StatePaused();
        UIManager.instance.DisplayLoseMenu();
    }
}
