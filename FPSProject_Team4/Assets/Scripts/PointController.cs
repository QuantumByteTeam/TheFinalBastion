using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// OLD
/*
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
        UIManager.instance.UpdatePointHP(health, healthOrig);
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
            UIManager.instance.UpdatePointHP(true);
            UIManager.instance.UpdatePointHP(health, healthOrig);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            UIManager.instance.UpdatePointHP(false);
        }
    }
}
*/

public class PointController : MonoBehaviour, Iinteractable, IDamageable, IRepairable
{
    [Header("----- Health -----")]
    public float health;
    public float healthOrig;

    [Header("----- Repair System -----")]
    [SerializeField] inventoryItem reqTool;
    [SerializeField] int cost;
    [SerializeField] float mult;
    
    [Header("----- Interactable -----")]
    [SerializeField] string prompt;

    private PlayerController player;

    void Start()
    {
        healthOrig = health;
        player = GameManager.instance.playerScript;
    }

    void Update()
    {
        if (health > healthOrig)
        {
            health = healthOrig;
        }
    }
    
    public string InteractionPrompt => prompt;

    public bool Interact(Interactor interactor)
    {
        if (health < healthOrig)
        {
            Repair();
        }
        
        return true;
    }

    void Repair()
    {
        if (player.inventory.hotbarInventory.ContainsKey(reqTool) == true)
        {
            RepairSystem(reqTool, cost, mult);
        }
    }
    
    public void takeDamage(float amount, float armorPen)
    {
        health -= amount;
        UIManager.instance.UpdatePointHP(health, healthOrig);
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
            UIManager.instance.UpdatePointHP(true);
            UIManager.instance.UpdatePointHP(health, healthOrig);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            UIManager.instance.UpdatePointHP(false);
        }
    }
    
    public void RepairSystem(inventoryItem requiredTool, int useCost = 1, float repairMult = 1)
    {
        health += reqTool.ShootDamage * repairMult;
        reqTool.ammoCount -= useCost;
        UIManager.instance.UpdatePointHP(true);
    }
}