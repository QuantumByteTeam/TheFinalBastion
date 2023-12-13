using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;

    [Header("----- Stats -----")]
    [SerializeField] float health;
    [SerializeField] int viewCone;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] bool shouldTargetPlayer;
    [SerializeField] bool shouldTargetPoint;
    [SerializeField] int pointTargetRange;

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
    
    bool isShooting;
    bool playerInRange;
    float randTime;
    float timer;
    Color colorOrig;

    //John
    public bool armor;

    void Start()
    {
        point = GameManager.instance.point;
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
        //else
        //{
        //    Roam
        //   timer += Time.deltaTime;
        //    if (timer >= walkTimer)
        //    {

        //        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;

        //        randomDirection += gameObject.transform.position;

        //        NavMeshHit hit;

        //        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, -1);
        //        agent.SetDestination(hit.position);
        //        timer = 0;
        //    }


        //}
    }


    bool canSeeTarget(Transform targetPos)
    {
        Vector3 targetDirection = targetPos.position - headPos.position;
        float angleToTarget = Vector3.Angle(targetDirection, transform.forward);

        Debug.DrawRay(headPos.position, targetDirection);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, targetDirection, out hit))
        {
            // FIXME: If point not hit, does not set destination
            if ((hit.collider.CompareTag("Player") && angleToTarget <= viewCone) || 
                hit.collider.CompareTag("Point") /*&& Vector3.Distance(transform.position, targetPos.position) <= pointTargetRange*/)
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
            //TODO: Figure out how to get enemies to spread around a target
            //else if (hit.collider.CompareTag("enemy"))
            //{
            //    SpreadOut(targetDirection);
            //}
        }

        return false;
    }

    //void SpreadOut(Vector3 targetPos)
    //{
    //    float angleIncrement = 360f / GameManager.instance.enemiesRemaining;

    //    float angle = transform.GetSiblingIndex() * angleIncrement;

    //    Vector3 spreadPosition = targetPos + Quaternion.Euler(0, angle, 0) * (transform.forward * agent.stoppingDistance);

    //    agent.SetDestination(spreadPosition);

    //}

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
            shouldTargetPoint = true;
        }
    }

    public void takeDamage(float amount, float armorPen)
    {
        if (armor)
        {
            health -= amount * armorPen;
        }
        else
        {
            health -= amount;
        }

        StartCoroutine(indicateDamage());

        if (health <= 0)
        {
            GameManager.instance.UpdateEnemyCount(-1);
            Destroy(gameObject);
        }
        else
        {
            shouldTargetPlayer = true;
            shouldTargetPoint = false;
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }

    IEnumerator indicateDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
