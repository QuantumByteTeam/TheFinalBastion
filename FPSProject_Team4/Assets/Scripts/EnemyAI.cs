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
    [SerializeField] bool canRoam;
    [SerializeField] int pointTargetRange;
    [SerializeField] float maxLookAngle;

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
        else if (canRoam && (!canSeeTarget(GameManager.instance.player.transform) || !canSeeTarget(point.transform)))
        {
            //Roam

            StartCoroutine(roam());

        }
    }


    bool canSeeTarget(Transform targetPos)
    {
        Quaternion originalRotation = firePos.rotation;
        Vector3 targetDirection = targetPos.position - headPos.position;
        float angleToTarget = Vector3.Angle(targetDirection, transform.forward);

        Debug.DrawRay(headPos.position, targetDirection);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, targetDirection, out hit))
        {
            // Calculate the direction to the target
            Vector3 fireToTarget = (targetPos.position - firePos.position).normalized;
            firePos.rotation = Quaternion.LookRotation(fireToTarget);


            // FIXME: If point not hit, does not set destination
            if ((hit.collider.CompareTag("Player") && angleToTarget <= viewCone))
            {
                agent.SetDestination(targetPos.position);
                //SpreadOut(targetDirection);

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
            else if (hit.collider.CompareTag("Enemy"))
            {
                StopCoroutine(shoot());
                isShooting = false;
            }
            else if (hit.collider.CompareTag("Point"))
            {
                agent.SetDestination(point.transform.position);

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }


                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget(targetDirection);
                }
            }
            
        }

        return false;
    }

    //TODO: Figure out how to get enemies to spread around a target
    //void SpreadOut(Vector3 targetPos)
    //{
    //    int uniqueIndex = gameObject.GetInstanceID();
    //    Debug.Log(uniqueIndex);

    //    float angleIncrement = 360f / GameManager.instance.enemiesRemaining;

    //    float angle = uniqueIndex * angleIncrement;

    //    Vector3 spreadPosition = targetPos + Quaternion.Euler(0, angle, 0) * (transform.forward * agent.stoppingDistance);

    //    agent.SetDestination(spreadPosition);

    //}

    void faceTarget(Vector3 targetDir)
    {
        Quaternion rot = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);

        // TODO: Setup vertical aiming

        //headPos.rotation = Quaternion.Euler(Vector3.Scale(rot.eulerAngles, new Vector3(1, 0, 0)));
        //Quaternion verticalRotation = Quaternion.Euler(rot.eulerAngles.x, 0f, 0f);
        //headPos.rotation = verticalRotation;
        //firePos.rotation = Quaternion.Euler(headPos.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    IEnumerator shoot()
    {
        isShooting = true;

        // Perform a ray cast
        Ray ray = new Ray(firePos.position, firePos.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object implements IDamagable
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();

            if (damageable != null)
            {
                // Instantiate and configure the bullet
                GameObject obj = Instantiate(bullet, firePos.position, firePos.rotation);
                Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>());
                Bullet bulletComp = obj.GetComponent<Bullet>();
                bulletComp.damageAmount = bulletDamage;
                bulletComp.speed = bulletSpeed;
                bulletComp.run();

                yield return new WaitForSeconds(fireRate);
            }
        }

        isShooting = false;
    }

    IEnumerator roam()
    {
        randTime = UnityEngine.Random.Range(0.5f, 3f);

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;

        randomDirection += gameObject.transform.position;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, -1);
        agent.SetDestination(hit.position);

        yield return new WaitForSeconds(randTime);
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
            GetComponent<LootBag>().InstantiateLoot(transform.position);
            Destroy(gameObject);
        }
        else
        {
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
