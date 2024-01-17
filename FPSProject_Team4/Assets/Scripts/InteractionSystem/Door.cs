using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Iinteractable
{

    [SerializeField] private string prompt;

    public string InteractionPrompt => prompt;

    public bool Interact(Interactor interactor)
    {
        /*var inventory = interactor.GetComponent<InventoryBool>(); //uncomment this code when starting on repair systems

        if (inventory == null) return false; //if inv is empty, do nothing

        if (inventory.HasWrench)
        {
            //repair code, add debug log code for a key is needed outside of these brackets when added. Add return true into here, and outside add return false
        }*/






        Debug.Log("Opening door!");
        return true;
    }





    
}
