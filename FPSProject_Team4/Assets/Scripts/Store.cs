using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour, Iinteractable
{
    [SerializeField] string interactPrompt;
    public GameObject itemDropPoint;

    public string InteractionPrompt => interactPrompt;

    public bool Interact(Interactor interactor)
    {
        GameManager.instance.accessedStore = this;
        UIManager.instance.DisplayStoreMenu();

        return true;
    }
}
