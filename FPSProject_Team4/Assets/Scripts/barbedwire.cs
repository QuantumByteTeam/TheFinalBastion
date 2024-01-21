using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Wire : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && !other.isTrigger)
        {
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            agent.speed = 0.5f;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy" && !other.isTrigger)
        {
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            agent.speed = 3.5f;
        }
    }
}