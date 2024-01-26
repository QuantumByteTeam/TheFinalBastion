using System.Collections;
using UnityEngine;

public class grenade : MonoBehaviour
{
    [SerializeField] float explosionDamage;
    [SerializeField] float explosionRadius;
    [SerializeField] float armorPen;
    [SerializeField] float knockbackModifier;
    [SerializeField] float throwForce;
    [SerializeField] AudioClip[] explosionSound;
    [SerializeField] Rigidbody rb;
    Collider[] enemies;
    IDamageable dmg;
    private void Start()
    {
        rb.velocity = rb.transform.forward * throwForce;
        StartCoroutine(fuse(5));
    }

    IEnumerator fuse(float time)
    {
        yield return new WaitForSeconds(time);

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
                            dmg.takeDamage(explosionDamage, armorPen);

                        }
                    }
                }

            }
        }

        Destroy(gameObject);

    }
}