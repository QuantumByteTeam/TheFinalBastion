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
    [SerializeField] GameObject menuOptions;
    public GameObject optionsActive;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuStore;
    [SerializeField] TMP_Text waveCountText;
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text coinCountText;
    public GameObject playerDamageScreen;
    public GameObject playerFlashScreen;
    public GameObject CraftingUI;

 

    public GameObject menuActive;

    [SerializeField] TMP_Text ammoCounterText;
    [SerializeField] TMP_Text reserveAmmoText;
    [SerializeField] GameObject reloadingText;

    //Interact UI Elements
    public GameObject InteractImage;
    public GameObject PromptText;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateBalance();
    }

    public void DisplayPausedMenu()
    {
        menuActive = menuPause;
        menuActive.SetActive(true);
    }

    public void DisplayOptionsMenu()
    {
        menuActive = menuOptions;
        menuOptions.SetActive(true);
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

    public void UpdateAmmo()
    {
        PlayerController playerCont = GameManager.instance.playerScript;
        if (playerCont.inventory.hotbarInventory.Count <= 0)
        {
            ammoCounterText.text = playerCont.ammoCount.ToString("0");
            reserveAmmoText.text = playerCont.ammoReserve.ToString("0");
        }
        else if (playerCont.inventory.hotbarInventory.ElementAt(playerCont.SelectedItem).Key.isGun)
        {
            ammoCounterText.text = playerCont.ammoCount.ToString("0");
            reserveAmmoText.text = playerCont.ammoReserve.ToString("0");
        }
        else
        {
            ammoCounterText.text = playerCont.inventory.hotbarInventory.ElementAt(playerCont.SelectedItem).Value.ToString();
            reserveAmmoText.text = "0";
        }
    }

    public void UpdateBalance()
    {
        coinCountText.text = GameManager.instance.coins.ToString("0");
    }

    public IEnumerator reloading(float time)
    {
        reloadingText.SetActive(true);
        yield return new WaitForSeconds(time);
        reloadingText.SetActive(false);
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

    
    public void exitToMainMenu()
    {
        SceneManager.LoadScene("TitleMenu");
    }



    public void blind()
    {
        StartCoroutine(playerBlind());
    }
    public IEnumerator playerBlind()
    {
        playerFlashScreen.SetActive(true);
        yield return new WaitForSeconds(5f);
        playerFlashScreen.SetActive(false);
    }

}
