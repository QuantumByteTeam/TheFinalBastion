using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSystem : MonoBehaviour, ISimpleInteractable, IDamageable, IRepairable
{
    [Header("----- Pipe System -----")]
    public bool isSteam;
    public bool isWater;
    public bool isGas;

    [Header("----- Effect GameObjects -----")]
    public GameObject waterHitEffect;
    public GameObject vapourHitEffect;
    
    [Header("----- Effect GameObjects -----")]
    [SerializeField] GameObject steamObject;
    [SerializeField] GameObject waterObject;
    [SerializeField] GameObject gasObject;
    
    [Header("----- Health -----")]
    public float health;
    public float healthOrig;

    [Header("----- Repair System -----")]
    [SerializeField] inventoryItem reqTool;
    [SerializeField] private int cost;
    [SerializeField] float mult;

    [Header("----- Interactable -----")]
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;
    
    private PlayerController player;
    
    // Start is called before the first frame update
    void Start()
    {
        healthOrig = health;
        player = GameManager.instance.playerScript;
    }

    // Update is called once per frame
    void Update()
    {
        if (health > healthOrig)
        {
            health = healthOrig;
        }

        if (gameObject.transform.childCount > 5)
        {
            RemoveExcess();
        }
    }

    void RemoveExcess()
    {
        for (int i = 0; i < 4; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.CompareTag("PipeHitEffect"))
            {
                StartCoroutine(RemoveChild(gameObject.transform.GetChild(i).gameObject));
            }
        }
    }

    IEnumerator RemoveChild(GameObject child)
    {
        child.GetComponent<ParticleSystem>().Stop();

        yield return new WaitUntil(() => !child.GetComponent<ParticleSystem>().IsAlive());
        
        Destroy(child);
    }

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
        health -= amount;
    }

    public void RepairSystem(inventoryItem requiredTool, int useCost = 1, float repairMult = 1)
    {
        health += reqTool.ShootDamage * repairMult;
        reqTool.ammoCount -= useCost;
    }
}
