using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public powerUpEffect powerUpEffect;
   
    private void OnTriggerEnter(Collider collision) //when player touches power up, it is destroyed then applied to player.
    {
        Destroy(gameObject, 10); //power up despawns after certain amount of time
        Destroy(gameObject);
        powerUpEffect.Apply(collision.gameObject);
    }

}
