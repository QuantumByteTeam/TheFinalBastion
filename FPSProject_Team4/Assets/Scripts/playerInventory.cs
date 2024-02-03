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

    public int Cloth = 0;
    public int Exp = 0;
    public int Circuits = 0;
    public int Scrap = 0;

    public void Add(inventoryItem item)
    {

    }

    public void AddMats(Loot loot)
    {
        switch (loot.name)
        {
            case "Currency":
                GameManager.instance.coins += loot.value;
                UIManager.instance.UpdateBalance();
                break;
            case "Cloth":
                Cloth++;
                break;
            case "Explosives":
                Exp++;
                break;
            case "Computer Chip":
                Circuits++;
                break;
            case "Metal Scrap":
                Scrap++;
                break;
            default:
                Debug.LogWarning("Invalid Loot Type");
                break;
        }
        UIManager.instance.updateMats();
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

            
            
            GameManager.instance.playerScript.ChangeItem();
            //UIManager.instance.updateHotbar(); <<<<<<<<<<<<<<< commented out
        }
        Debug.LogWarning(DictionarySize());
        GameManager.instance.playerScript.SelectedItem = 0;
        GameManager.instance.playerScript.ChangeItem();
        UIManager.instance.updateHotbar();
    }

    public void drop(int index)
    {
        inventoryItem item = hotbarInventory.ElementAt(index).Key;



        PlayerController player = GameManager.instance.playerScript;
        GameObject newItem = GameManager.Instantiate(item.droppedItem, Camera.main.transform.position + (Camera.main.transform.forward * 3), Camera.main.transform.rotation);
        if (item.isGun)
        {
            newItem.GetComponent<GunPickup>().gun.ammoCount = player.ammoCount;
            newItem.GetComponent<GunPickup>().gun.ammoReserve = player.ammoReserve;
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