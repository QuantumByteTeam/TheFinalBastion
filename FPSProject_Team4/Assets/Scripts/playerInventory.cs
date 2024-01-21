using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class playerInventory // : MonoBehaviour
{

    //public static playerInventory instance;

    public Dictionary<inventoryItem, int> hotbarInventory = new Dictionary<inventoryItem, int>();

    public void Add(inventoryItem item)
    {

    }

    public void Remove(int index)
    {
        hotbarInventory[hotbarInventory.ElementAt(index).Key]--;
        inventoryItem item = hotbarInventory.ElementAt(index).Key;
        Debug.Log("Item Removed");
        if (hotbarInventory[hotbarInventory.ElementAt(index).Key] == 0)
        {
            hotbarInventory.Remove(item);
            Debug.Log(hotbarInventory.ContainsKey(item));
            Debug.Log(DictionarySize());
            if (GameManager.instance.playerScript.SelectedItem >= hotbarInventory.Count && hotbarInventory.Count > 0)
            {
                GameManager.instance.playerScript.SelectedItem = hotbarInventory.Count-1;
            }
            
            GameManager.instance.playerScript.ChangeItem();
            UIManager.instance.updateHotbar();
        }
    }

    public int DictionarySize()
    {
        return hotbarInventory.Count;
    }
}