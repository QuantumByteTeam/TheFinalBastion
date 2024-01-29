using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    public int secondsUntilDestroy;
    public int damageAmount;
    public int speed;
    public float armorPen;

    public void run()
    {
        rb.velocity = transform.forward * this.speed;
        Destroy(gameObject, secondsUntilDestroy);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamageable damage = other.GetComponent<IDamageable>();

        if (tag == "Player" && other.tag == "Point")
        {
            Destroy(gameObject);
        }

        if (damage != null)
        {
            damage.takeDamage(damageAmount, armorPen);
        }

        Destroy(gameObject);
    }
}
