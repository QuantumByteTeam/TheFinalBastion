using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSystem : MonoBehaviour, ISimpleInteractable, IDamageable, IRepairable
{
    [Header("----- Pipe System -----")]
    public bool isSmoke;
    public bool isWater;
    public bool isGas;

    [Header("----- Hit Effect GameObjects -----")]
    public GameObject waterHitEffect;
    public GameObject vapourHitEffect;
    
    [Header("----- Effect GameObjects -----")]
    public bool effectCreated;
    [SerializeField] GameObject smokeObject;
    [SerializeField] GameObject waterObject;
    [SerializeField] GameObject gasObject;
    
    [Header("----- Health -----")]
    public float health;
    public float healthOrig;
    public bool fullHealth;
    public bool startDamaged;

    [Header("----- Repair System -----")]
    [SerializeField] inventoryItem reqTool;
    [SerializeField] private int cost;
    [SerializeField] float mult;

    [Header("----- Interactable -----")]
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;
    
    private PlayerController player;
    private Transform effectSpawnLoc;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!startDamaged)
        {
            healthOrig = health;
        }
        else
        {
            healthOrig = health * 2;
        }
        
        player = GameManager.instance.playerScript;
        effectSpawnLoc = gameObject.transform.GetChild(0).transform;

        if (mult == 0)
        {
            mult = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health >= healthOrig)
        {
            health = healthOrig;

            if (!fullHealth)
            {
                fullHealth = true;
                RemoveAllHitEffects();
            }
        }

        if (health < 0)
        {
            health = 0;
        }

        if (!effectCreated && health < (healthOrig * 0.8))
        {
            createEffect();
        }
        
        if (gameObject.transform.childCount > 5)
        {
            RemoveExcess();
        }
    }

    void createEffect()
    {
        effectCreated = true;
        
        if (isWater)
        {
            Instantiate(waterObject, effectSpawnLoc.position, effectSpawnLoc.rotation, effectSpawnLoc);
        }
        else if (isSmoke)
        {
            Instantiate(smokeObject, effectSpawnLoc.position, effectSpawnLoc.rotation, effectSpawnLoc);
        }
        else if (isGas)
        {
            Instantiate(gasObject, effectSpawnLoc.position, effectSpawnLoc.rotation, effectSpawnLoc);
        }
    }

    void RemoveExcess()
    {
        for (int i = 1; i < 5; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.CompareTag("PipeHitEffect"))
            {
                StartCoroutine(RemoveExcessChild(gameObject.transform.GetChild(i).gameObject));
            }
        }
    }

    void RemoveAllHitEffects()
    {
        for (int i = 1; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.CompareTag("PipeHitEffect"))
            {
                StartCoroutine(RemoveExcessChild(gameObject.transform.GetChild(i).gameObject));
            }
        }
    }

    IEnumerator RemoveExcessChild(GameObject child)
    {
        child.GetComponent<ParticleSystem>().Stop();

        yield return new WaitUntil(() => !child.GetComponent<ParticleSystem>().IsAlive());
        
        Destroy(child);
    }

    public void SimpleInteract(SimpleInteractor simpleInteractor)
    {
        Debug.Log("Click Click");
        
        if (health < healthOrig)
        {
            Repair();
        }
    }

    void Repair()
    {
        Debug.Log("Repairing...");
        
        if (player.inventory.hotbarInventory.ContainsKey(reqTool) == true)
        {
            Debug.Log("Repairing...Start!");
            RepairSystem(reqTool, cost, mult);
        }
    }

    public void takeDamage(float amount, float armorPen)
    {
        fullHealth = false;
        health -= amount;
    }

    public void RepairSystem(inventoryItem requiredTool, int useCost = 1, float repairMult = 1)
    {
        health += reqTool.ShootDamage * repairMult;
        reqTool.ammoCount -= useCost;
    }
}
