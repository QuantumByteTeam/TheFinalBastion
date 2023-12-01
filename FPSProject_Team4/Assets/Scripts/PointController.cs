using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour, IDamageable
{
    [SerializeField] int health;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void takeDamage(int amount)
    {
        health -= amount;
    }
}
