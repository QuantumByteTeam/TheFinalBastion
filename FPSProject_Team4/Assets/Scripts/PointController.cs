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

public class PointController : MonoBehaviour, ISimpleInteractable, IDamageable, IRepairable
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

    [Header("----- Is Attacked -----")]
    public bool isAttacked;
    private float timer;

    private bool playerOnPoint;
    private PlayerController player;

    void Start()
    {
        healthOrig = health;
        player = GameManager.instance.playerScript;
        CoreManager.instance.RegisterCore(gameObject);

    }

    void Update()
    {
        if (health > healthOrig)
        {
            health = healthOrig;
        }

        if (isAttacked)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
        }

        if (timer > 10)
        {
            isAttacked = false;
        }
    }
    
    public string InteractionPrompt => prompt;

    public void SimpleInteract(SimpleInteractor simpleInteractor)
    {
        if (health < healthOrig)
        {
            Repair();
        }
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
        if (!isAttacked)
        {
            isAttacked = true;
        }
        
        health -= amount;
        GameManager.instance.score -= (int)amount;
        UIManager.instance.UpdateScore();

        if (playerOnPoint)
        {
            UIManager.instance.UpdatePointHP(health, healthOrig);
        }
        
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
            playerOnPoint = true;
            UIManager.instance.UpdatePointHP(true);
            UIManager.instance.UpdatePointHP(health, healthOrig);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerOnPoint = false;
            UIManager.instance.UpdatePointHP(false);
        }
    }
    
    public void RepairSystem(inventoryItem requiredTool, int useCost = 1, float repairMult = 1)
    {
        health += reqTool.ShootDamage * repairMult;
        GameManager.instance.score += (int)(reqTool.ShootDamage* repairMult*0.5);
        UIManager.instance.UpdateScore();
        reqTool.ammoCount -= useCost;
        UIManager.instance.UpdatePointHP(true);
    }
}