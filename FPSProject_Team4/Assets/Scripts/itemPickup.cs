using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour
{
    [SerializeField] inventoryItem item;

    bool triggerSet; //code that fixes a bug with picking up multiple of the same type of gun (activates trigger multiple times)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerSet) //use interfaces (like IDamage) to change if u want entities to pick things up/take dmg etc.
        {
            triggerSet = true;
            //give stats to player
            GameManager.instance.playerScript.GetGunStats(item);
            Destroy(gameObject);
            Debug.Log("Item picked up");
        }


    }


}