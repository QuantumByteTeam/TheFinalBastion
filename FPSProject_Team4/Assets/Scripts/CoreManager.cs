using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    public static CoreManager instance;
    public List<GameObject> cores;

    void Awake()
    {
        instance = this;
    }

    public void RegisterCore(GameObject obj)
    {
        if (obj.GetComponent<PointController>()) // TODO: Uncomment this check once Core is implemented
        {
            cores.Add(obj);
        }
    }

    public List<GameObject> GetAllCores()
    {
        return cores;
    }

    public GameObject GetClosestToPosition(Vector3 position)
    {
        GameObject closestObj = null;
        float closestDist = float.MaxValue;

        foreach (GameObject obj in cores)
        {
            float distance = Vector3.Distance(obj.transform.position, position);

            if (distance < closestDist)
            {
                closestObj = obj;
                closestDist = distance;
            }
        }

        return closestObj;
    }

    public GameObject GetRandomPoint()
    {
        int i = Random.Range(0, cores.Count);
        return cores[i];
    }
}
