using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour, IDamageable
{
    [SerializeField] int health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount)
    {
        health -= amount;
    }
}
