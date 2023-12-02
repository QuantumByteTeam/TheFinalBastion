using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour, IDamageable
{
    public int health;
    
    public int healthOrig;

    void Start()
    {
        healthOrig = health;
        UIManager.instance.UpdatePointHP();
    }

    public void takeDamage(int amount)
    {
        health -= amount;
        UIManager.instance.UpdatePointHP();

        if (health <= 0)
        {
            //player dies
            GameManager.instance.YouLose();
        }
    }
}
