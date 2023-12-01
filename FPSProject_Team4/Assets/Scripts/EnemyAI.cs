using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("----- Stats -----")]
    [SerializeField] int health;

    [Header("----- Controls -----")]
    [SerializeField] bool targetPlayer;
    [SerializeField] bool targetPoint;

    [Header("----- Weapon Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePos;
    [SerializeField] float fireRate;
    [SerializeField] int bulletDamage;
    [SerializeField] int bulletSpeed;

    bool isShooting;

    void Start()
    {
        // Send info to gamemanager
    }

    void Update()
    {
        if (!isShooting)
        {
            StartCoroutine(shoot());
        }
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

    public void takeDamage(int amount)
    {
        health -= amount;
    }
}
