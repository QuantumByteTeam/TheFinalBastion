using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnValidation
    {
        [Tooltip("The max angle for the floor."), Range(0, 359)] public int maxFloorAngle;
        [Tooltip("The max number of attempts the spawn validation step can take before failing."), Range(0, 1000)] public int maxAttempts;
    }

    [Tooltip("Spawn Validation Items")] public SpawnValidation spawnValidation;

    // Make sure the spawnSize does not go negative when changed in the inspector to prevent weird issues
    private void OnValidate()
    {
        transform.localScale = new Vector3(Mathf.Max(0, transform.localScale.x), Mathf.Max(0, transform.localScale.y), Mathf.Max(0, transform.localScale.z));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, transform.localScale);

        Gizmos.color = Color.green;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 4;
        Gizmos.DrawRay(transform.position, direction);
    }

    /// <summary>
    /// Trigger this spawner to spawn the specified enemy.
    /// </summary>
    /// <returns>True if the enemy was properly spawned, else false.</returns>
    public bool TriggerSpawner(GameObject enemy)
    {
        Vector3 spawnPos;
        bool safePosition = false;
        int attempts = 0;

        do
        {
            spawnPos = GetRandomPosWithinArea();

            RaycastHit hit;
            Vector3 rayDirection = Vector3.down;
            float distance = Mathf.Abs(transform.localScale.y - (transform.position.y + transform.localScale.y / 2 - spawnPos.y));

            // Check if there is a floor below the enemy
            if (Physics.Raycast(spawnPos, rayDirection, out hit, distance))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);

                // make sure the angle of the point below the random spawn position is not more than the defined max floor angle
                if (angle <= spawnValidation.maxFloorAngle)
                {
                    Vector3 enemySize = enemy.GetComponent<Renderer>().bounds.size;
                    float halfEnemyHeight = enemySize.y / 2;
                    float radius = Mathf.Max(enemySize.x / 2, enemySize.z / 2);

                    spawnPos.y = hit.point.y + halfEnemyHeight;

                    // Make sure enemy does not spawn in a wall
                    if (!Physics.CheckSphere(spawnPos, radius))
                    {
                        safePosition = true;
                    }
                }
            }

            attempts++;

        } while (!safePosition && attempts <= spawnValidation.maxAttempts);

        if (safePosition)
        {
            Instantiate(enemy, spawnPos, transform.rotation);
        }
        else
        {
            Debug.LogWarning("Unable to find safe spawn position within " + spawnValidation.maxAttempts + " attempts.");
        }

        return safePosition;
    }

    /// <summary>
    /// Get a Random Position within a rectangular area
    /// </summary>
    /// <returns>Random Position within the defined spawn area.</returns>
    private Vector3 GetRandomPosWithinArea()
    {
        float x = Random.Range(transform.position.x - (transform.localScale.x / 2), transform.position.x + (transform.localScale.x / 2));
        float y = Random.Range(transform.position.y - (transform.localScale.y / 2), transform.position.y + (transform.localScale.y / 2));
        float z = Random.Range(transform.position.z - (transform.localScale.z / 2), transform.position.z + (transform.localScale.z / 2));

        return new Vector3(x, y, z);
    }
}