using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingMechanic : MonoBehaviour
{
    [SerializeField] float explosionRadius;
    [SerializeField] float explosionDamage;
    [SerializeField] GameObject explosionEffect;

    bool hasExploded;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        // explosion effect
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Deal damage to nearby objects within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.takeDamage(explosionDamage, 1f);
            }
        }

        Destroy(gameObject);
    }
}
