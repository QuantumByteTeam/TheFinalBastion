using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] WaveManager waveManager;

    public GameObject player;
    public GameObject playerSpawnPos;
    public GameObject point;

    public PlayerController playerScript;
    public PointController pointScript;

    public bool isPaused;
    public int enemiesRemaining;
    public int waveCount;
    float timescaleOG;
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        point = GameObject.FindWithTag("Point");
        pointScript = point.GetComponent<PointController>();
        playerSpawnPos = GameObject.FindWithTag("PlayerSpawn");
        timescaleOG = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && UIManager.instance.menuActive == null)
        {
            StatePaused();
            UIManager.instance.DisplayPausedMenu();
        }
    }

    public void StatePaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpaused()
    {
        isPaused = !isPaused;
        Time.timeScale = timescaleOG;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        UIManager.instance.CloseActiveMenu();
    }

    public void UpdateEnemyCount(int amount)
    {
        enemiesRemaining += amount;
        enemiesRemaining = Mathf.Max(enemiesRemaining, 0);
        UIManager.instance.UpdateRemainingEnemies();

        if (enemiesRemaining <= 0)
        {
            StartCoroutine(waveManager.StartWave());
        }
    }

    public void UpdateWaveCount(int amount)
    {
        waveCount += amount;
        UIManager.instance.UpdateWaveCount();
    }

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
