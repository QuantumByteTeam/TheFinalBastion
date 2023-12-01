using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;

    public int secondsUntilDestroy;
    public int damageAmount;
    public int speed;

    public void run()
    {
        rigidbody.velocity = transform.forward * this.speed;
        Destroy(gameObject, secondsUntilDestroy);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamageable damage = other.GetComponent<IDamageable>();

        if (damage != null)
        {
            damage.takeDamage(damageAmount);
        }

        Destroy(gameObject);
    }
}
