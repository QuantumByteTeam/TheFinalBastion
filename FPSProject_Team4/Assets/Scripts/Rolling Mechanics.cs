using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingMechanics : MonoBehaviour
{
    [SerializeField] float rollingSpeed;
    [SerializeField] float damageAmount;
    [SerializeField] float knockbackForce;
    [SerializeField] float knockbackDuration;

    [SerializeField] Collider damageCollider;
    [SerializeField] Rigidbody rigidbody;

    EnemyAI enemyAI;

    bool canRoll;

    private void Start()
    {
        canRoll = true;
        damageCollider.enabled = false;
        enemyAI = GetComponent<EnemyAI>();
    }

    void EnableCollision()
    {
        damageCollider.enabled = true;
    }
    void DisableCollision()
    {
        damageCollider.enabled = false;
    }

    // OnTriggerEnter is called when the rolling enemy collides with another collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Deal damage to the player upon collision
            other.GetComponent<IDamageable>().takeDamage(damageAmount, 1f);
        }
    }

    public void InitiateRollingAttack()
    {
        if (canRoll)
        {
            StartCoroutine(RollingAttack());
        }
    }

    IEnumerator RollingAttack()
    {
        canRoll = false;
        EnableCollision();
        StartRolling(rollingSpeed);

        yield return new WaitForSeconds(1.0f);

        StopRolling();
        ApplyKnockback();

        yield return new WaitForSeconds(2);

        canRoll = true;
    }

    void StartRolling(float speed)
    {
        //Include enemy AI seeing and chasing behavior in here somehow
        //Nav mesh will go towards player in the if statement in enemy ai when it sees the player
    }

    void StopRolling()
    {
        rigidbody.velocity = Vector3.zero;
        //Include Nav mesh agent things here as well or instead of this
    }

    void ApplyKnockback()
    {
        Vector3 knockbackDirection = (GameManager.instance.player.transform.position - transform.position).normalized;
        rigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

        StartCoroutine(StopRollingForDuration());
    }

    IEnumerator StopRollingForDuration()
    {
        StopRolling();
        yield return new WaitForSeconds(knockbackDuration);
        StartRolling(rollingSpeed);
    }
}
