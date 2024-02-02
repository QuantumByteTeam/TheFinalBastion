using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] Image playerHPBar;
    [SerializeField] GameObject pointHPElement;
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
    [SerializeField] TMP_Text scoreCountText;
    public GameObject playerDamageScreen;
    public GameObject playerFlashScreen;
    public GameObject CraftingUI;
    [SerializeField] GameObject selectionBox;
    [SerializeField] GameObject[] hotbarSlots = new GameObject[9];
    [SerializeField] Sprite emptySlotSprite;
    public GameObject menuActive;
    [SerializeField] TMP_Text ammoCounterText;
    [SerializeField] TMP_Text reserveAmmoText;
    [SerializeField] public GameObject reloadingText;
    public GameObject smokeVignette;

    [SerializeField] TMP_Text ReactorText;
    [SerializeField] TMP_Text LifeSupportText;
    [SerializeField] TMP_Text ComputerText;
    //Interact UI Elements
    public GameObject InteractImage;
    public GameObject PromptText;
    public GameObject InteractImageS;
    public GameObject PromptTextS;

    PointController reactor;
    PointController oxygen;
    PointController computer;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateBalance();
        reactor = GameObject.Find("Reactor_Core").GetComponent<PointController>();
        oxygen = GameObject.Find("Atmospheric_Regulator").GetComponent<PointController>();
        computer = GameObject.Find("Cray-1_Supercomputer").GetComponent<PointController>();
    }


    public void DisplayPausedMenu()
    {
        menuActive = menuPause;
        menuActive.SetActive(true);
    }

    public void DisplayOptionsMenu()
    {
        menuOptions.SetActive(true);
    }

    /*public void closeOptionsMenu()
    {
        menuOptions.SetActive(false);
        menuActive.SetActive(true);
    }*/

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

    public void CloseOptionsMenu()
    {
        menuOptions.SetActive(false);
    }

    public void CloseStoreMenu()
    {
        GameManager.instance.ActiveUnpause();
        Resume();
    }

    public void UpdatePlayerHP()
    {
        PlayerController playerCont = GameManager.instance.playerScript;
        playerHPBar.fillAmount = (float)playerCont.HP / playerCont.HPOriginal;
    }

    public void UpdatePointHP(bool pointHPVisible)
    {
        pointHPElement.SetActive(pointHPVisible);
    }

    public void UpdatePointHP(float pointHP, float pointHPMax)
    {
        float ratio = pointHP / pointHPMax;
        pointHPBar.fillAmount = ratio;
        if (ratio <= 0.25f)
        {
            pointHPBar.color = Color.red;
        }
        else if (ratio <= 0.5f)
        {
            pointHPBar.color = Color.yellow;
        }
        else
        {
            pointHPBar.color = Color.green;
        }
    }


    public void UpdatePointIndicators()
    {
        
        if (reactor.isAttacked)
        {
            ReactorText.gameObject.SetActive(true);
            ReactorText.color = Color.red;
        }
        else if (reactor.isTargeted)
        {
            //Debug.Log("Reactor Targeted");
            ReactorText.gameObject.SetActive(true);
            ReactorText.color = Color.yellow;
        }
        else
        {
            ReactorText.gameObject.SetActive(false);
            ReactorText.color = Color.white;
        }

        if (computer.isAttacked)
        {
            ComputerText.gameObject.SetActive(true);
            ComputerText.color = Color.red;
        }
        else if (computer.isTargeted)
        {
            //Debug.Log("Reactor Targeted");
            ComputerText.gameObject.SetActive(true);
            ComputerText.color = Color.yellow;
        }
        else
        {
            ComputerText.gameObject.SetActive(false);
            ComputerText.color = Color.white;
        }

        if (oxygen.isAttacked)
        {
            LifeSupportText.gameObject.SetActive(true);
            LifeSupportText.color = Color.red;
        }
        else if (oxygen.isTargeted)
        {
            //Debug.Log("Life Support Targeted");
            LifeSupportText.gameObject.SetActive(true);
            LifeSupportText.color = Color.yellow;
        }
        else
        {
            LifeSupportText.gameObject.SetActive(false);
            LifeSupportText.color = Color.white;
        }

    }
    public void UpdateWaveCount()
    {
        WaveManager wave = GameManager.instance.waveScript;
        waveCountText.text = wave.GetCurrentWave().ToString("0");
    }

    public void UpdateRemainingEnemies()
    {
        enemyCountText.text = GameManager.instance.enemiesRemaining.ToString("0");
        if (GameManager.instance.enemiesRemaining <= 5 && GameManager.instance.enemiesRemaining > 0)
        {
            enemyCountText.gameObject.SetActive(true);
        }
        else
        {
            enemyCountText.gameObject.SetActive(false);
        }
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

    public void UpdateScore()
    {
        scoreCountText.text = GameManager.instance.score.ToString("0");
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
        GameManager.instance.resetGuns();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpaused();
    }

    public void quit()
    {
        GameManager.instance.resetGuns();
        Application.Quit();
    }

    public void updateSelection(int selection)
    {
        selectionBox.transform.localPosition = new UnityEngine.Vector3((selection * (478f / 8)) - 239f, -480, 0);
        updateHotbar();
    }

    public void updateHotbar()
    {
        int j = GameManager.instance.playerScript.inventory.hotbarInventory.Count;
        for (int i = 0; i < j; i++)
        {
            hotbarSlots[i].GetComponent<Image>().sprite = GameManager.instance.playerScript.inventory.hotbarInventory.ElementAt(i).Key.returnIcon();
        }
        for (int i = j; i < 9; i++)
        {
            hotbarSlots[i].GetComponent<Image>().sprite = emptySlotSprite;
        }
        UpdateAmmo();
    }
    public void exitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");       
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
