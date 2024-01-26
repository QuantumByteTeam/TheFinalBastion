using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class turret : MonoBehaviour, IDamageable
{
    [SerializeField] public int viewCone;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] public float range;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePos;
    [SerializeField] public Transform headPos;
    [SerializeField] public float fireRate;
    [SerializeField] public int bulletDamage;
    [SerializeField] int bulletSpeed;
    [SerializeField] public float armorPen;

    [SerializeField] public float hp;

    int EnemyInRange;
    public bool isShooting;

    Collider[] colliders;

    void Update()
    {
        if (EnemyInRange > 0 && canSeeTarget(closestEnemy()))
        {

        }
    }

    public void takeDamage(float amount, float armorPen)
    {
        hp -= amount;
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private Transform closestEnemy()
    {
        colliders = Physics.OverlapSphere(transform.position, range);

        Transform closest = null;
        float temp = range+1;

        for (int i = 0; i < colliders.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, colliders[i].transform.position);
            if (colliders[i].tag == "Enemy" && dist < temp)
            {
                closest = colliders[i].transform;
                temp = dist;
            }
        }
        return closest;
    }

    bool canSeeTarget(Transform targetPos)
    {
        if (targetPos == null)
        {
            return false;
        }
        Quaternion originalRotation = firePos.rotation;
        Vector3 targetDirection = targetPos.position - headPos.position;
        float angleToTarget = Vector3.Angle(targetDirection, transform.forward);

        Debug.DrawRay(headPos.position, targetDirection);
        RaycastHit hit;

        if (Physics.Raycast(headPos.position, targetDirection, out hit))
        {
            Vector3 fireToTarget = (targetPos.position - firePos.position).normalized;
            firePos.rotation = Quaternion.LookRotation(fireToTarget);

            if (hit.collider.CompareTag("Enemy") && angleToTarget <= viewCone)
            {
                
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                faceTarget(targetDirection);

                return true;
            }

        }
        return false;
    }
    void faceTarget(Vector3 targetDir)
    {
        Quaternion rot = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
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
                bulletComp.armorPen = armorPen;
                bulletComp.speed = bulletSpeed;
                bulletComp.run();

                yield return new WaitForSeconds(fireRate);
            }
        }

        isShooting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyInRange++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyInRange--;
        }
    }
}
