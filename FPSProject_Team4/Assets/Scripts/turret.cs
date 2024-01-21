//using UnityEngine;


//public class turret : MonoBehaviour
//{

//    [SerializeField] GameObject bullet;
//    [SerializeField] Transform firePos;
//    [SerializeField] public Transform headPos;
//    [SerializeField] public float fireRate;
//    [SerializeField] public int bulletDamage;
//    [SerializeField] int bulletSpeed;

//    bool canSeeTarget(Transform targetPos)
//    {
//        Quaternion originalRotation = firePos.rotation;
//        Vector3 targetDirection = targetPos.position - headPos.position;
//        float angleToTarget = Vector3.Angle(targetDirection, transform.forward);

//        Debug.DrawRay(headPos.position, targetDirection);
//        RaycastHit hit;

//        if (Physics.Raycast(headPos.position, targetDirection, out hit))
//        {
//            // Calculate the direction to the target
//            Vector3 fireToTarget = (targetPos.position - firePos.position).normalized;
//            firePos.rotation = Quaternion.LookRotation(fireToTarget);


//            // FIXME: If point not hit, does not set destination
//            if ((hit.collider.CompareTag("Player") && angleToTarget <= viewCone))
//            {
//                agent.SetDestination(targetPos.position);
//                //SpreadOut(targetDirection);
//                if (!isShooting)
//                {
//                    StartCoroutine(shoot());
//                }

//                if (agent.remainingDistance < agent.stoppingDistance)
//                {
//                    faceTarget(targetDirection);
//                }

//                return true;
//            }
//            else if (hit.collider.CompareTag("Enemy"))
//            {
//                StopCoroutine(shoot());
//                isShooting = false;
//            }
//            else if (hit.collider.CompareTag("Point"))
//            {
//                agent.SetDestination(point.transform.position);

//                if (!isShooting)
//                {
//                    StartCoroutine(shoot());
//                }
//                if (agent.remainingDistance < agent.stoppingDistance)
//                {
//                    faceTarget(targetDirection);
//                }
//            }

//        }
//        return false;
//    }
//    void faceTarget(Vector3 targetDir)
//    {
//        Quaternion rot = Quaternion.LookRotation(targetDir);
//        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
//    }

//    IEnumerator shoot()
//    {
//        isShooting = true;

//        // Perform a ray cast
//        Ray ray = new Ray(firePos.position, firePos.forward);
//        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit))
//        {
//            // Check if the hit object implements IDamagable
//            IDamageable damageable = hit.collider.GetComponent<IDamageable>();

//            if (damageable != null)
//            {
//                // Instantiate and configure the bullet
//                GameObject obj = Instantiate(bullet, firePos.position, firePos.rotation);
//                Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>());
//                Bullet bulletComp = obj.GetComponent<Bullet>();
//                bulletComp.damageAmount = bulletDamage;
//                bulletComp.speed = bulletSpeed;
//                bulletComp.run();

//                yield return new WaitForSeconds(fireRate);
//            }
//        }

//        isShooting = false;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Enemy"))
//        {
//            EnemyInRange++;
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Enemy"))
//        {
//            EnemyInRange--;
//        }
//    }
//}
