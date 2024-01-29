using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] 

public class Loot : ScriptableObject
{
    [Header("--- Loot Table ---")]
    public string lootName;
    public int dropChance; //1 - 100% chance
    public Texture lootSprite;

    public Loot(string lootName, int dropChance) //Constructor. Can be used in other classes
    {
        this.lootName = lootName;
        this.dropChance = dropChance;
    }

    private void OnCollisionEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {

        }
    }
}
