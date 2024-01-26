using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class flashbang : MonoBehaviour
{
    [SerializeField] float blindTime;
    [SerializeField] float explosionRadius;
    [SerializeField] float throwForce;
    [SerializeField] AudioClip[] explosionSound;
    [SerializeField] Rigidbody rb;
    Collider[] enemies;
    IDamageable dmg;
    private void Start()
    {
        rb.velocity = rb.transform.forward * throwForce;
        StartCoroutine(fuse(2.5f));
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
                            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                            Vector3 targetDirection = transform.position - enemy.headPos.position;
                            float angleToTarget = Vector3.Angle(targetDirection, enemy.transform.forward);

                            if (angleToTarget < enemy.viewCone/2)
                            {
                                
                                float dist = Vector3.Distance(transform.position, enemy.headPos.position);

                                enemy.stopShoot(blindTime);
                                
                                //blind(enemy, dist);
                            }
                        }
                    }
                }

            }
            else if (enemies[i].tag == "Player")
            {
                RaycastHit hit;

                Vector3 targetDirection = transform.position - Camera.main.transform.position;
                float angleToTarget = Vector3.Angle(targetDirection, Camera.main.transform.forward);

                if (Physics.Raycast(Camera.main.transform.position, transform.position-Camera.main.transform.position, out hit, explosionRadius))
                {
                    if (hit.collider.tag == "Flashbang" && angleToTarget <= 45)
                    {
                        UIManager.instance.blind();
                    }
                }
            }
        }

        Destroy(gameObject);

    }

    


}