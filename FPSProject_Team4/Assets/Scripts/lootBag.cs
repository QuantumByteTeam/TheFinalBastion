using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    public GameObject itemDropsPrefab;
    public List<Loot> lootList = new List<Loot>();
    
    Loot GetDroppedItem()
    {
        int randomNumber = Random.Range(1, 101); //1 - 100
        List<Loot> possibleDrops = new List<Loot>();
        foreach(Loot item in lootList)
        {
            if(randomNumber <= item.dropChance)
            {
                possibleDrops.Add(item);
                
            }
        }
        if(possibleDrops.Count > 0)
        {
            Loot droppedItem = possibleDrops[Random.Range(0, possibleDrops.Count)];
            return droppedItem;
        }
        Debug.Log("No Loot Dropped");
        return null;
    }

    public void InstantiateLoot(Vector3 spawnPosition)
    {
        Loot droppedItem = GetDroppedItem();
        if(droppedItem != null)
        {
            GameObject lootGameObject = Instantiate(itemDropsPrefab, spawnPosition, Quaternion.identity);
            lootGameObject.GetComponent<SpriteRenderer>().sprite = droppedItem.lootSprite;

            //can add item drop flair and visual effects here if able
        }
    }
}
