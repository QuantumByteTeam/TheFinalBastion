using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Mine : MonoBehaviour
{
    [SerializeField] float explosionDamage;
    [SerializeField] float explosionRadius;
    [SerializeField] float armorPen;
    [SerializeField] float knockbackModifier;
    [SerializeField] AudioClip[] explosionSound;
    Collider[] enemies;
    IDamageable dmg;

    private bool exploded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !other.isTrigger)
        {
            
            enemies = Physics.OverlapSphere(transform.position, explosionRadius);
                
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].tag == "Enemy")
                {
                    dmg = null;
                    dmg = enemies[i].GetComponent<IDamageable>();

                    if (dmg != null)
                    {

                        RaycastHit hit;

                        if (Physics.Raycast(transform.position, enemies[i].transform.position - transform.position, out hit, explosionRadius))
                        {

                            if (hit.collider.tag == "Enemy")
                            {

                                Debug.LogError("Hit");

                                dmg.takeDamage(explosionDamage, armorPen);

                            }
                        }
                    }
                    
                }
            }

                exploded = true;
                Destroy(gameObject);

        }
        
        if (other.tag == "Player" || other.tag == "EnemyBullet")
        {
            return;
        }
        
    }

}