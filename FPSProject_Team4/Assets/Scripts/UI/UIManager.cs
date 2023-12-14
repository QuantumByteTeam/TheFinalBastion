using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] Image playerHPBar;
    [SerializeField] Image pointHPBar;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuStore;
    [SerializeField] TMP_Text waveCountText;
    [SerializeField] TMP_Text enemyCountText;
    public GameObject playerDamageScreen;

    public GameObject menuActive;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void DisplayPausedMenu()
    {
        menuActive = menuPause;
        menuActive.SetActive(true);
    }

    public void DisplayLoseMenu()
    {
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void DisplayWinMenu()
    {
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void DisplayStoreMenu()
    {
        menuActive = menuStore;
        menuActive.SetActive(true);
    }

    public void CloseActiveMenu()
    {
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void UpdatePlayerHP()
    {
        PlayerController playerCont = GameManager.instance.playerScript;
        playerHPBar.fillAmount = (float)playerCont.HP / playerCont.HPOriginal;
    }

    public void UpdatePointHP()
    {
        PointController pointCont = GameManager.instance.pointScript;
        pointHPBar.fillAmount = (float)pointCont.health / pointCont.healthOrig;
    }

    public void UpdateWaveCount()
    {
        WaveManager wave = GameManager.instance.waveScript;
        waveCountText.text = wave.currentWave.ToString("0");
    }

    public void UpdateRemainingEnemies()
    {
        enemyCountText.text = GameManager.instance.enemiesRemaining.ToString("0");
    }

    public void Resume()
    {
        GameManager.instance.StateUnpaused();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpaused();
    }

    public void quit()
    {
        Application.Quit();
    }
}
