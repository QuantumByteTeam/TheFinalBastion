using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;

    [Header("----- Stats -----")]
    [SerializeField] int health;
    [SerializeField] int viewCone;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] bool shouldTargetPlayer;
    [SerializeField] bool shouldTargetPoint;
    [SerializeField] int walkRadius;
    [SerializeField] float walkTimer;

    [Header("----- Weapon Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePos;
    [SerializeField] Transform headPos;
    [SerializeField] float fireRate;
    [SerializeField] int bulletDamage;
    [SerializeField] int bulletSpeed;

    [Header("----- Target -----")]
    public GameObject point;

    public MobSpawner originSpawner;
    
    bool isShooting;
    bool playerInRange;
    bool pointInRange;
    float randTime;
    float timer;
    Color colorOrig;

    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.UpdateEnemyCount(1);
    }

    void Update()
    {
        if (shouldTargetPlayer && playerInRange && canSeeTarget(GameManager.instance.player.transform))
        {

        } 
        else if (shouldTargetPoint && canSeeTarget(point.transform))
        {

        }
        else
        {
            // Roam
            timer += Time.deltaTime;
            if (timer >= walkTimer)
            {
                
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;

                randomDirection += gameObject.transform.position;

                NavMeshHit hit;

                NavMesh.SamplePosition(randomDirection, out hit, walkRadius, -1);
                agent.SetDestination(hit.position);
                timer = 0;
            }

            
        }
    }


    bool canSeeTarget(Transform targetPos)
    {
        Vector3 targetDirection = targetPos.position - headPos.position;
        float angleToTarget = Vector3.Angle(targetDirection, transform.forward);

        Debug.DrawRay(headPos.position, targetDirection);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, targetDirection, out hit))
        {
            if ((hit.collider.CompareTag("Player") && angleToTarget <= viewCone) || hit.collider.CompareTag("Point"))
            {
                agent.SetDestination(targetPos.position);

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget(targetDirection);
                }

                return true;
            }
        }

        return false;
    }

    void faceTarget(Vector3 targetDir)
    {
        Quaternion rot = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);

        // TODO: Setup vertical aiming
        //headPos.rotation = Quaternion.Euler(Vector3.Scale(rot.eulerAngles, new Vector3(1, 0, 0)));
    }

    IEnumerator shoot()
    {
        isShooting = true;

        GameObject obj = Instantiate(bullet, firePos.position, transform.rotation);
        Bullet bulletComp = obj.GetComponent<Bullet>();
        bulletComp.damageAmount = bulletDamage;
        bulletComp.speed = bulletSpeed;
        bulletComp.run();

        yield return new WaitForSeconds(fireRate);

        isShooting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void takeDamage(int amount)
    {
        health -= amount;
        
        StartCoroutine(indicateDamage());

        if (health <= 0)
        {
            GameManager.instance.UpdateEnemyCount(-1);
            originSpawner.UpdateEnemyKilled();
            Destroy(gameObject);
        }
    }

    IEnumerator indicateDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
