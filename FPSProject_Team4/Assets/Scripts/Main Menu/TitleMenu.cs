using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Place Holder Main Map");
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
        //whoever is working on Options menu, this will be called when the options button is clicked
    }

}
