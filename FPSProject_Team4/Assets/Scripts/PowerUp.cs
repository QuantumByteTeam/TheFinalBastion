using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public powerUpEffect powerUpEffect;
    [SerializeField] public GameObject powerupprefab;
    public List<powerUpEffect> buffList = new List<powerUpEffect>();


    private void OnTriggerEnter(Collider collision) //when player touches power up, it is destroyed then applied to player.
    {

       /* if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            powerUpEffect.Apply(collision.gameObject);
            Debug.Log("powerup");
        }*/
    }

    private void Awake()
    {
        StartCoroutine(fadePowerup());
    }
    /*public void drop(Vector3 spawnPosition)
    {
        GameObject orb = Instantiate(powerupprefab, spawnPosition, Quaternion.identity);
        orb.GetComponent<PowerUp>().powerUpEffect = buffList[Random.Range(0, 2)];
        orb.GetComponent<PowerUp>().enabled = true;
    }*/
    IEnumerator fadePowerup()
    {
        yield return new WaitForSeconds(7);
        Object.Destroy(this.gameObject);
    }


}
