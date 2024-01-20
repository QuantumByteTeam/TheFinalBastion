using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, Iinteractable
{
    [SerializeField] private string prompt;

    private void Update()
    {
   /* if (Input.GetKeyDown(KeyCode.Q))
        {
            
            Cursor.lockState = CursorLockMode.None;
            UIManager.instance.CraftingUI.SetActive(false);
            Cursor.visible = false;
        }*/

        
    }



    public string InteractionPrompt => prompt;
   

    public bool Interact(Interactor interactor)
    {
        UIManager.instance.CraftingUI.SetActive(true);
        Debug.Log("Opening Crafting Menu");
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        


        return true;
    }




    





}
