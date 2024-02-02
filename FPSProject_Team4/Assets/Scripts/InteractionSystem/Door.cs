using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, ISimpleInteractable
{
    [SerializeField, Range(0, 1)] float doorBreakProb = 0.25f;
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;

    private Renderer rendererD; //I had to change the name because renderer and collider were already reserved for the unity components and it caused errors to leave it as renderer
    private Collider colliderD;

    private bool isBroken;
    private bool isClosed;

    void Awake()
    {
        isBroken = false;
        isClosed = false;
        rendererD = gameObject.GetComponent<Renderer>();
        colliderD = gameObject.GetComponent<Collider>();
    }

    public void SimpleInteract(SimpleInteractor interactor)
    {
        var inventory = interactor.GetComponent<InventoryBool>();

        if (inventory == null) return; //if inv is empty, do nothing

        if (isBroken)
        {
            if (inventory.HasWrench)
            {
                //repair code, add debug log code for a key is needed outside of these brackets when added. Add return true into here, and outside add return false
                isBroken = false;
            }
        }
        else
        {
            if (isClosed)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
    }

    public void DetermineIfBroken()
    {
        float random = Random.Range(0f, 1f);

        if (random <= doorBreakProb)
        {
            isBroken = true;
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        isClosed = false;
        rendererD.enabled = true;
        colliderD.enabled = true;
    }

    public void CloseDoor()
    {
        isClosed = true;
        rendererD.enabled = false;
        colliderD.enabled = false;
    }

   
}
