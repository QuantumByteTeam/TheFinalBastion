using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, ISimpleInteractable
{
    [SerializeField, Range(0, 1)] float doorBreakProb = 0.25f;
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;

    private Renderer renderer;
    private Collider collider;

    private bool isBroken;
    private bool isClosed;

    void Awake()
    {
        isBroken = false;
        isClosed = false;
        renderer = gameObject.GetComponent<Renderer>();
        collider = gameObject.GetComponent<Collider>();
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
        renderer.enabled = true;
        collider.enabled = true;
    }

    public void CloseDoor()
    {
        isClosed = true;
        renderer.enabled = false;
        collider.enabled = false;
    }
}
