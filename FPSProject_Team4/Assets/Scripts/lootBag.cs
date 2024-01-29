using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    [SerializeField] public GameObject itemDropsPrefab;
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
            var lootGameObject = Instantiate(itemDropsPrefab, spawnPosition, Quaternion.identity);
            //lootPrefabscript lootGameObject = Instantiate(itemDropsPrefab, spawnPosition, Quaternion.identity) as lootPrefabscript;
            lootGameObject.GetComponent<MeshRenderer>().material.mainTexture = droppedItem.lootSprite;
            lootPrefabscript lps = lootGameObject.GetComponent<lootPrefabscript>();
            lps.loot = droppedItem;
            //lootGameObject.GetComponent<> = droppedItem;
            //can add item drop flair and visual effects here if able
        }
    }
}
