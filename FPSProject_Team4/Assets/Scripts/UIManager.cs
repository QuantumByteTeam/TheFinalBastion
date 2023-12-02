using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject player;
    [SerializeField] TMP_Text HPText;
    [SerializeField] GameObject PointHPBar;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject point;

    [SerializeField] Image pointHPImage;

    float timeScale;
    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        timeScale= Time.timeScale;
        instance= this;
        player = GameManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        //need to access the HP of the point
        UIManager.instance.pointHPImage.fillAmount = point.HP / point.HPOriginal;

        //need to update the HP integer label with the HP of the player, a bar is not used, but a label, i cannot access the player HP from here

        //Might want to update these labels (wave and enemy included) not on every frame
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
