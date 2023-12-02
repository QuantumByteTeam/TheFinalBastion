using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public GameObject player;
    public GameObject playerSpawnPos;
    public PlayerController playerScript;

    public Image playerHPBar;
    
    [SerializeField] GameObject activeMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text waveCountText;

    public bool isPaused;
    int enemiesRemaining;
    int waveCount;
    float timescaleOG;
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPos = GameObject.FindWithTag("PlayerSpawn");
        timescaleOG = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            StatePaused();
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);
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
        activeMenu.SetActive(false);
        activeMenu = null;
    }

    public void UpdateEnemyCount(int amount)
    {
        enemiesRemaining += amount;
        enemiesRemaining = Mathf.Max(enemiesRemaining, 0);
        enemyCountText.text = enemiesRemaining.ToString("0");
    }

    public void UpdateWaveCount(int amount)
    {
        waveCount = amount;
        waveCountText.text = waveCount.ToString("0");
    }

    public void YouWin()
    {
        StatePaused();
        activeMenu = winMenu;
        activeMenu.SetActive(true);
    }

    public void YouLose()
    {
        StatePaused();
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }
}
