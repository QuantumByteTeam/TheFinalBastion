using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RollingMechanics : MonoBehaviour
{
    [SerializeField] public float rollingSpeed;
    [SerializeField] float damageAmount;
    [SerializeField] float knockbackForce;
    [SerializeField] float knockbackDuration;

    [SerializeField] Collider damageCollider;
    [SerializeField] Rigidbody body;

    EnemyAI enemyAI;

    public bool canRoll;

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
        if (other.CompareTag("Player") || other.CompareTag("Point"))
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

        yield return new WaitForSeconds(1.0f);


        ApplyKnockback();

        DisableCollision();

        yield return new WaitForSeconds(knockbackDuration);

        canRoll = true;
    }



    void StopRolling()
    {
        enemyAI.agent.velocity = Vector3.zero;
        //Include Nav mesh agent things here as well or instead of this
    }

    void ApplyKnockback()
    {
        // Calculate the knockback direction
        Vector3 knockbackDirection = (transform.position - GameManager.instance.player.transform.position).normalized;

        body.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

        StartCoroutine(StopRollingForDuration());
    }

    IEnumerator StopRollingForDuration()
    {
        StopRolling();
        yield return new WaitForSeconds(knockbackDuration);
        enemyAI.agent.speed = rollingSpeed;
    }
}
