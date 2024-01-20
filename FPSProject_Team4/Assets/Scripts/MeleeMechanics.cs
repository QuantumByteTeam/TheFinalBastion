using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMechanics : MonoBehaviour
{
    [SerializeField] float meleeRange;
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeCooldown;
    [SerializeField] Collider damageCollider;

    bool isAttacking;
    private EnemyAI enemyAI;

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        damageCollider.enabled = false;
        isAttacking = false;
    }

    // Method to initiate the melee attack
    public void InitiateMeleeAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(MeleeAttack());
        }
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;

        EnableDamageCollider();

        // Wait for a short duration to detect collisions
        yield return new WaitForSeconds(0.2f);

        PerformMeleeAttack(meleeDamage);

        DisableDamageCollider();

        yield return new WaitForSeconds(meleeCooldown);

        isAttacking = false;
    }

    // Method to perform the melee attack (deal damage to the player)
    void PerformMeleeAttack(float damage)
    {
        // Example: Deal melee damage to the player or target
        GameManager.instance.player.GetComponent<IDamageable>().takeDamage(damage, 1f);
    }

    void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

}
