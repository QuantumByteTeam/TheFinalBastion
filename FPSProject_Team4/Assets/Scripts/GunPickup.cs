using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] public inventoryItem gun;

    bool triggerSet; //code that fixes a bug with picking up multiple of the same type of gun (activates trigger multiple times)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerSet && GameManager.instance.playerScript.inventory.hotbarInventory.Count < 9) 
        {
            triggerSet = true;
            //give stats to player
            GameManager.instance.playerScript.GetGunStats(gun);
            UIManager.instance.updateSelection(GameManager.instance.playerScript.SelectedItem);
            Destroy(gameObject);
        }


    }


}

