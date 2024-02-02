using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInteractor : MonoBehaviour //this is more or less a copy paste of the interaction code but this will not active pause
{ 
[SerializeField] private Transform interactionPoint;
[SerializeField] private float interactionPointRadius = 2;
[SerializeField] private LayerMask interactableMask;


private readonly Collider[] colliders = new Collider[3];

[SerializeField] private int _numFound; //not needed but nice to see in inspector, shows number of colliders found


    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, colliders, interactableMask);
        //finds everything within this pos, this rad, thats apart of the mask, and fills the collider array 

        if (_numFound > 0) //if something is within the interaction sphere, assigns it to first spot of colliders array
        {
            
            UIManager.instance.InteractImageS.SetActive(true);
            UIManager.instance.PromptTextS.SetActive(true);

            var SimpleInteractable = colliders[0].GetComponent<ISimpleInteractable>();


            if (SimpleInteractable != null && Input.GetKeyDown(KeyCode.E)) //this code is setup for a way to differentiate from a simple interaction that doesnt need a paues and a complex one
            {
                SimpleInteractable.SimpleInteract(this);
                UIManager.instance.InteractImageS.SetActive(false);
                UIManager.instance.PromptTextS.SetActive(false);
            }

        }
        else
        {
            UIManager.instance.InteractImageS.SetActive(false); //turns off interact image (placeolder is blue rectangle)
            UIManager.instance.PromptTextS.SetActive(false); //turnsoff the interact text
        }
    }

    private void OnDrawGizmos() //draws sphere that overlaps with objs in the world (interactable layer)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionPointRadius);
    }
}
