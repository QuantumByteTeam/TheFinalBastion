using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public powerUpEffect powerUpEffect;
   
    private void OnTriggerEnter(Collider collision) //when player touches power up, it is destroyed then applied to player.
    {
        Destroy(gameObject);
        powerUpEffect.Apply(collision.gameObject);
    }

    private void Awake()
    {
        StartCoroutine(fadePowerup());
    }

    IEnumerator fadePowerup()
    {
        yield return new WaitForSeconds(7);
        Object.Destroy(this.gameObject);
    }

}
