using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/DmgBuff")]

public class dmgPickUp : powerUpEffect
{
    public int amount;

    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().damageModifier *= amount;
    }
}
