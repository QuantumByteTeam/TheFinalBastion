using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/SpeedBuff")]
public class speedPickUp : powerUpEffect
{
   public float amount;

    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().PlayerSpeed += amount;
    }

}
