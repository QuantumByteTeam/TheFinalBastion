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
    [SerializeField] ParticleSystem explosion;
    [SerializeField] ParticleSystem shockwave;
    [SerializeField] GameObject explosionPosition;
    [SerializeField] AudioClip clip;
    Collider[] enemies;
    IDamageable dmg;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !other.isTrigger)
        {
            float v = (float)((UIManager.instance.getVolume() + 80) / 80);
            AudioSource.PlayClipAtPoint(clip, transform.position, v);
            Instantiate(explosion, transform.position, Quaternion.LookRotation(Vector3.up, Vector3.up));
            Instantiate(shockwave, transform.position, Quaternion.LookRotation(Vector3.up, Vector3.up));

            enemies = Physics.OverlapSphere(explosionPosition.transform.position, explosionRadius);
                
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].tag == "Enemy")
                {
                    dmg = null;
                    dmg = enemies[i].GetComponent<IDamageable>();

                    if (dmg != null)
                    {



                        //RaycastHit[] hits;

                        //hits = Physics.RaycastAll(transform.position, enemies[i].transform.position - transform.position, Vector3.Distance(transform.position, enemies[i].transform.position));

                        //foreach (RaycastHit rayhit in  hits)
                        //{
                        //    if (rayhit.collider.tag != "Enemy" && rayhit.collider.tag != "Player")
                        //    {
                        //        return;
                        //    }
                        //}

                        //dmg.takeDamage(explosionDamage, armorPen);

                        RaycastHit hit;

                        if (Physics.Raycast(explosionPosition.transform.position, enemies[i].transform.position - explosionPosition.transform.position, out hit, explosionRadius))
                        {
                            Debug.DrawRay(explosionPosition.transform.position, enemies[i].transform.position - explosionPosition.transform.position, Color.red, explosionRadius);
                            if (hit.collider.tag == "Enemy")
                            {
                                Debug.Log("Damaged Enemy");
                                dmg.takeDamage(explosionDamage, armorPen);
                            }
                        }
                    }

                }
            }

                Destroy(gameObject);

        }
        
        if (other.tag == "Player" || other.tag == "EnemyBullet")
        {
            return;
        }
        
    }

}