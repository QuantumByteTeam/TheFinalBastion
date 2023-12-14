using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class EnemyProbability : ScriptableObject
{
    public GameObject enemy;
    public float probability;
    public int coinKillReward;
}
