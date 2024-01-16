using System.Collections;
using System.Collections.Generic;
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
            var interactable = colliders[0].GetComponent<Iinteractable>();

            if (interactable != null && Input.GetKeyDown(KeyCode.E)) 
            {
                interactable.Interact(this);
            }
        }

    }

    private void OnDrawGizmos() //draws sphere that overlaps with objs in the world (interactable layer)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionPointRadius);
    }




}
