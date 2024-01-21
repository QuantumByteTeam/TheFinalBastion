using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class playerInventory //: MonoBehaviour
{

    //public static playerInventory instance;

    public Dictionary<inventoryItem, int> hotbarInventory = new Dictionary<inventoryItem, int>();

    public void Add(inventoryItem item)
    {

    }

    public void Remove(int index)
    {
        //Debug.Log(hotbarInventory.ElementAt(index).Key);
        hotbarInventory[hotbarInventory.ElementAt(index).Key]--;
        inventoryItem item = hotbarInventory.ElementAt(index).Key;
        
        if (hotbarInventory[hotbarInventory.ElementAt(index).Key] == 0)
        {
            GameManager.instance.playerScript.swap = true;
            hotbarInventory.Remove(item);
            if (GameManager.instance.playerScript.SelectedItem >= hotbarInventory.Count && hotbarInventory.Count > 0)
            {
                GameManager.instance.playerScript.SelectedItem = hotbarInventory.Count-1;
            }

            if(DictionarySize() <= 0)
            {
                GameManager.instance.playerScript.SelectedItem = 0;
                GameManager.instance.playerScript.ChangeItem();
                UIManager.instance.updateHotbar();
            }
            
            GameManager.instance.playerScript.ChangeItem();
            //UIManager.instance.updateHotbar(); <<<<<<<<<<<<<<< commented out
        }
    }

    public void drop(int index)
    {
        inventoryItem item = hotbarInventory.ElementAt(index).Key;

        

        GameObject newItem = GameManager.Instantiate(item.droppedItem, Camera.main.transform.position + (Camera.main.transform.forward * 3), Camera.main.transform.rotation);
        if (item.isGun)
        {
            //gunStats newGunStats = newItem.gameObject.GetComponent<gunStats>();
            //newGunStats.ammoCount = item.ammoCount;
            //newGunStats.ammoMag = item.ammoMag;
            //newGunStats.ammoReserve = item.ammoReserve;
            //newGunStats.ammoCount = item.ammoCount;
        }

        Remove(index);

        if (GameManager.instance.playerScript.SelectedItem >= hotbarInventory.Count && hotbarInventory.Count > 0)
        {
            GameManager.instance.playerScript.SelectedItem = hotbarInventory.Count - 1;
        }

        
        GameManager.instance.playerScript.ChangeItem();
        UIManager.instance.updateHotbar();
    }

    public int DictionarySize()
    {
        return hotbarInventory.Count;
    }
}