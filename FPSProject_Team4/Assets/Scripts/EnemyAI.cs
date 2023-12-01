using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("----- Stats -----")]
    [SerializeField] int health;

    [Header("----- Controls -----")]
    [SerializeField] bool targetPlayer;
    [SerializeField] bool targetPoint;

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
