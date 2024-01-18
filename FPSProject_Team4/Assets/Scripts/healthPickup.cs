using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/HealthPickUp")]
public class healthPickup : powerUpEffect
{
    public float amount;

    public override void Apply(GameObject target) //target is whatever the buff will apply to, the player
    {
        target.GetComponent<PlayerController>().HP += amount;
    }

}
