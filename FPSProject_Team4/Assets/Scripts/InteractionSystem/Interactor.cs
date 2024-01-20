using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask interactableMask;
    

    private readonly Collider[] colliders = new Collider[3];

    [SerializeField] private int _numFound; //not needed but nice to see in inspector, shows number of colliders found


    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, colliders, interactableMask);
        //finds everything within this pos, this rad, thats apart of the mask, and fills the collider array 

        if (_numFound > 0) //if something is within the interaction sphere, assigns it to first spot of colliders array
        {
            UIManager.instance.InteractImage.SetActive(true);
            UIManager.instance.PromptText.SetActive(true);
            
            var interactable = colliders[0].GetComponent<Iinteractable>();
            
            
            if (interactable != null && Input.GetKeyDown(KeyCode.E)) //this code is setup for a way to differentiate from a simple interaction that doesnt need a paues and a complex one
            {
                interactable.Interact(this);
                GameManager.instance.ActivePaused(); //pauses the game for ALL menu's accessed by interaction
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {

                Cursor.lockState = CursorLockMode.None;
                UIManager.instance.CraftingUI.SetActive(false); //turns off crafting UI
                Cursor.visible = false;
                GameManager.instance.ActiveUnpause(); //unpauses the game for ALL menu's accessed by interaction
            }
            else if (interactable != null && Input.GetKeyDown(KeyCode.E))
            {
                interactable.Interact(this); //for simple things like doors, has NO pause effects
            }
                }
                else
                {
                UIManager.instance.InteractImage.SetActive(false); //turns off interact image (placeolder is blue rectangle)
                UIManager.instance.PromptText.SetActive(false); //turnsoff the interact text
                }

    }

    private void OnDrawGizmos() //draws sphere that overlaps with objs in the world (interactable layer)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionPointRadius);
    }




}
