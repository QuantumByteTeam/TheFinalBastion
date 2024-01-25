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
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] Renderer model;

    [Header("----- Stats -----")]
    [SerializeField] public float health;
    [SerializeField] public int viewCone;
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
    [SerializeField] public Transform headPos;
    [SerializeField] public float fireRate;
    [SerializeField] public int bulletDamage;
    [SerializeField] int bulletSpeed;

    [Header("----- Target -----")]
    public GameObject point;

    public bool isBuffed = false;
    bool isShooting;
    bool isRoaming;
    bool playerInRange;
    bool isRoller;
    bool isExploder;
    float randTime;
    float timer;
    [SerializeField] int reward;
    Color colorOrig;
    RollingMechanics roller;
    ExplodingMechanic exploder;

    //John
    public bool armor;

    void Start()
    {
        point = GameManager.instance.point;
        colorOrig = model.material.color;
        GameManager.instance.UpdateEnemyCount(1);
        if (GetComponent<RollingMechanics>() != null)
        {
            roller = GetComponent<RollingMechanics>();
            isRoller = true;
        }
        if (GetComponent<ExplodingMechanic>() != null)
        {
            exploder = GetComponent<ExplodingMechanic>();
            isExploder = true;
        }
    }

    void Update()
    {
        if (shouldTargetPlayer && playerInRange && canSeeTarget(GameManager.instance.player.transform))
        {
            
        } 
        else if (shouldTargetPoint && canSeeTarget(point.transform))
        {
            Debug.Log("point seen");
        }
        else
        {
            //Roam
            if (canRoam && !isRoaming)
            {
                StartCoroutine(roam());
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
            // Calculate the direction to the target
            Vector3 fireToTarget = (targetPos.position - firePos.position).normalized;
            firePos.rotation = Quaternion.LookRotation(fireToTarget);


            // FIXME: If point not hit, does not set destination
            if ((hit.collider.CompareTag("Player") && angleToTarget <= viewCone))
            {
                if (isRoaming)
                {
                    StopCoroutine(roam());
                    isRoaming = false;
                }
                agent.SetDestination(targetPos.position);
                //SpreadOut(targetDirection);

                if (!isRoller && !isExploder)
                {
                    if (!isShooting)
                    {
                        StartCoroutine(shoot());
                    }
                }

                if (!isRoller)
                {
                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        faceTarget(targetDirection);
                    }
                }
                
                if (isRoller)
                {
                    agent.speed = roller.rollingSpeed;
                    if (agent.remainingDistance == agent.stoppingDistance)
                    {
                        roller.InitiateRollingAttack();
                    }
                }
                

                
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                StopCoroutine(shoot());
                isShooting = false;
            }
            else if (hit.collider.CompareTag("Point") && angleToTarget <= viewCone)
            {
                if (isRoaming)
                {
                    StopCoroutine(roam());
                    isRoaming = false;
                }
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
            return true;

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

    public void StartRolling(float speed)
    {
        //Include enemy AI seeing and chasing behavior in here somehow
        //Nav mesh will go towards player in the if statement in enemy ai when it sees the player

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

                Destroy(obj);
            }

            
        }

        isShooting = false;
    }

    IEnumerator roam()
    {
        isRoaming = true;

        randTime = UnityEngine.Random.Range(0.5f, 3f);

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;

        randomDirection += gameObject.transform.position;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, -1);
        agent.SetDestination(hit.position);

        // Wait until the agent has reached its destination
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        yield return new WaitForSeconds(randTime);

        isRoaming = false;
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
            //LootBag lootBag = GetComponent<LootBag>();
            //if (lootBag)
            //{
            //    lootBag.InstantiateLoot(transform.position);
            //    GameManager.instance.score += reward;
            //    GameManager.instance.coins += reward;
            //}
            
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

    public bool stopShoot(float time)
    {
        StopCoroutine(blind(0));
        StopCoroutine(shoot());
        isShooting = true;
        StartCoroutine(blind(time));
        return true;
    }

    public IEnumerator blind(float time)
    {
        
            
        Debug.LogError("Enemy Stopped Shooting");
        float temp = fireRate;
        int temp2 = targetFaceSpeed;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        fireRate = 100;
        Debug.LogError("Enemy firerate: 5");
        agent.speed = 0;
        Debug.LogError("Enemy Speed: 0");
        targetFaceSpeed = 0;
        yield return new WaitForSeconds(time);
        targetFaceSpeed = temp2;
        fireRate = temp;
        isShooting = false;
        agent.speed = 3.5f;
        
    }
}
