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
    }

    public void takeDamage(float amount, float armorPen)
    {
        health -= amount;
        //UIManager.instance.UpdatePointHP(health, healthOrig); <<<<<<<<<<<<<< commented out
        GameManager.instance.score -= (int)amount;
        if (health <= 0)
        {
            //player dies
            GameManager.instance.YouLose();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //UIManager.instance.UpdatePointHP(true);
            //UIManager.instance.UpdatePointHP(health, healthOrig);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //UIManager.instance.UpdatePointHP(false); <<<<<<<<<<< commented out
        }
    }
}
