using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ammoPickup : MonoBehaviour
{

    bool triggerSet; //code that fixes a bug with picking up multiple of the same type of gun (activates trigger multiple times)
    bool used;
    // Start is called before the first frame update
    void Start()
    {
        used = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerSet)
        {
            triggerSet = true;
            //give stats to player
            foreach (inventoryItem item in GameManager.instance.playerScript.inventory.hotbarInventory.Keys)
            {
                if (item.isGun)
                {
                    used = true;
                    item.ammoCount = item.ammoMag;
                    item.ammoReserve = item.ammoReserveDefault;
                }
            }
            GameManager.instance.playerScript.ammoCount = GameManager.instance.playerScript.inventory.hotbarInventory.ElementAt(GameManager.instance.playerScript.SelectedItem).Key.ammoMag;
            GameManager.instance.playerScript.ammoReserve = GameManager.instance.playerScript.inventory.hotbarInventory.ElementAt(GameManager.instance.playerScript.SelectedItem).Key.ammoReserveDefault;
            UIManager.instance.UpdateAmmo();
            if (used)
            {
                Destroy(gameObject);
            }
            else
            {
                used = false;
            }
        }


    }


}