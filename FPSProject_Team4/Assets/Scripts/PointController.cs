using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool isTargeted;
    public float timer = 5;
    public float timer1 = 5;

    private bool playerOnPoint;
    private PlayerController player;

    [SerializeField] ParticleSystem damageParticles;
    [SerializeField] GameObject particleLocation;
    ParticleSystem particles;

    void Start()
    {
        healthOrig = health;
        player = GameManager.instance.playerScript;
        CoreManager.instance.RegisterCore(gameObject);
        particles = Instantiate(damageParticles, particleLocation.transform.position, Quaternion.LookRotation(Vector3.up, Vector3.up));
        particles.Stop();
    }

    void Update()
    {
        if (health > healthOrig)
        {
            health = healthOrig;
        }

        timer += Time.deltaTime;

        if (timer > 3)
        {
            isAttacked = false;
        }

        timer1 += Time.deltaTime;

        if (timer1 > 0.1)
        {
            isTargeted = false;
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
        timer = 0;
        health -= amount;
        GameManager.instance.score -= (int)amount;
        UIManager.instance.UpdateScore();

        if (playerOnPoint)
        {
            UIManager.instance.UpdatePointHP(health, healthOrig);
        }

        if (health < (healthOrig/3))
        {
            particles.Play();
        }

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

        if (health >= (healthOrig/3))
        {
            particles.Stop();
        }
    }
}