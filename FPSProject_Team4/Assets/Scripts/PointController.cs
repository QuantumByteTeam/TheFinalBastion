using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour, IDamageable
{
    public float health;
    
    public float healthOrig;

    void Start()
    {
        healthOrig = health;
        UIManager.instance.UpdatePointHP();
    }

    public void takeDamage(float amount, float armorPen)
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
