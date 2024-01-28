using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] GameObject menuOptions;
    public void StartGame()
    {
        SceneManager.LoadScene("Place Holder Main Map");
        Debug.Log("crash");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial Level");

    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");


    }

    public void Options()
    {
     //Need to open options menu in title menu, would not be trivial
    }

}
