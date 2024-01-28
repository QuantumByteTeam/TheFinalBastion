using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBuff : MonoBehaviour
{
    [Header("----- Core -----")] 
    [SerializeField] int cooldown;
    [SerializeField] float buffRadius;
    Collider[] enemies;
    
    [Header("----- Buff - Armor -----")]
    [SerializeField] bool enableBuffArmor;
    [SerializeField] int armorDuration;
    
    [Header("----- Buff - HP Regen -----")]
    [SerializeField] bool enableBuffRegen;
    [SerializeField] float healthLimitMult;
    [SerializeField] float regenAmount;
    [SerializeField] float regenRate;
    [SerializeField] int regenDuration;
    
    [Header("----- Buff - Speed -----")]
    [SerializeField] bool enableBuffSpeed;
    [Range(1, 3)][SerializeField] float speedMult;
    [SerializeField] int speedDuration;
    
    [Header("----- Buff - Damage -----")]
    [SerializeField] bool enableBuffDamage;
    [SerializeField] int damageMult;
    [SerializeField] int damageDuration;
    
    [Header("----- Buff - Atk Speed -----")]
    [SerializeField] bool enableBuffAtkSpeed;
    [SerializeField] float atkSpeedMult;
    [SerializeField] int atkSpeedDuration;
    
    bool onCooldown;
    private List<string> bufflist = new List<string>();

    private void Start()
    {
        if (enableBuffArmor) { bufflist.Add("armor"); }
        if (enableBuffRegen) { bufflist.Add("regen"); }
        if (enableBuffSpeed) { bufflist.Add("speed"); }
        if (enableBuffDamage) { bufflist.Add("damage"); }
        if (enableBuffAtkSpeed) { bufflist.Add("atkspeed"); }
    }

    void Update()
    {
        if (!GameManager.instance.isActivePaused)
        {
            if (!GameManager.instance.isPaused)
            {
                if (!onCooldown)
                {
                    StartCoroutine(CastBuff());
                }
            }
        }
    }
    
    IEnumerator CastBuff()
    {
        onCooldown = true;
        
        enemies = Physics.OverlapSphere(transform.position, buffRadius, 1<<19);
        
        foreach (Collider i in enemies)
        {
            string buff = bufflist[Random.Range(0, bufflist.Count)];
            
            switch (buff)
            {
                case "armor":
                    StartCoroutine(BuffArmor(i));
                    break;
                case "regen":
                    StartCoroutine(BuffRegen(i));
                    break;
                case "speed":
                    StartCoroutine(BuffSpeed(i));
                    break;
                case "damage":
                    StartCoroutine(BuffDamage(i));
                    break;
                case "atkspeed":
                    StartCoroutine(BuffAtkSpeed(i));
                    break;
            }
        }

        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }

    IEnumerator BuffArmor(Collider enemy)
    {
        if (!enemy.GetComponent<EnemyAI>().armor && !enemy.GetComponent<EnemyAI>().isBuffed)
        {
            enemy.GetComponent<EnemyAI>().isBuffed = true;
            enemy.GetComponent<EnemyAI>().armor = true;
            yield return new WaitForSeconds(armorDuration);
            enemy.GetComponent<EnemyAI>().armor = false;
            enemy.GetComponent<EnemyAI>().isBuffed = false;
        }
    }

    IEnumerator BuffRegen(Collider enemy)
    {
        float healthLimit = enemy.GetComponent<EnemyAI>().health * healthLimitMult;
        WaitForSeconds regenInterval = new WaitForSeconds(regenRate);

        if (!enemy.GetComponent<EnemyAI>().isBuffed)
        {
            enemy.GetComponent<EnemyAI>().isBuffed = true;
            
            while (enemy.GetComponent<EnemyAI>().health <= healthLimit)
            {
                yield return regenInterval;
                enemy.GetComponent<EnemyAI>().health += regenAmount;
            }
        }
        
        yield return new WaitForSeconds(regenDuration);
        enemy.GetComponent<EnemyAI>().isBuffed = false;
    }

    IEnumerator BuffSpeed(Collider enemy)
    {
        float origSpeed = enemy.GetComponent<NavMeshAgent>().speed;

        if (!enemy.GetComponent<EnemyAI>().isBuffed)
        {
            enemy.GetComponent<EnemyAI>().isBuffed = true;
            enemy.GetComponent<NavMeshAgent>().speed *= speedMult;
        }

        yield return new WaitForSeconds(speedDuration);
        enemy.GetComponent<NavMeshAgent>().speed = enemy.GetComponent<EnemyAI>().origSpeed;
        enemy.GetComponent<EnemyAI>().isBuffed = false;
    }

    IEnumerator BuffDamage(Collider enemy)
    {
        int origDamage = enemy.GetComponent<EnemyAI>().bulletDamage;

        if (!enemy.GetComponent<EnemyAI>().isBuffed)
        {
            enemy.GetComponent<EnemyAI>().isBuffed = true;
            enemy.GetComponent<EnemyAI>().bulletDamage *= damageMult;
        }

        yield return new WaitForSeconds(damageDuration);
        enemy.GetComponent<EnemyAI>().bulletDamage = origDamage;
        enemy.GetComponent<EnemyAI>().isBuffed = false;
    }
    
    IEnumerator BuffAtkSpeed(Collider enemy)
    {
        float origAtkSpeed = enemy.GetComponent<EnemyAI>().fireRate;

        if (!enemy.GetComponent<EnemyAI>().isBuffed)
        {
            enemy.GetComponent<EnemyAI>().isBuffed = true;
            enemy.GetComponent<EnemyAI>().fireRate *= atkSpeedMult;
        }

        yield return new WaitForSeconds(atkSpeedDuration);
        enemy.GetComponent<EnemyAI>().fireRate = origAtkSpeed;
        enemy.GetComponent<EnemyAI>().isBuffed = false;
    }
}
