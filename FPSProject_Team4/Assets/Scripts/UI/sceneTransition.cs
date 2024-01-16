using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTransition : MonoBehaviour
{
    [SerializeField] int nextSceneIndex;
    public static int nextScene;
    public int NS;
    public GameObject levelDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nextScene = NS;
            //sceneManager.Instance.loadScene(nextSceneIndex);
        }
    }


}
 