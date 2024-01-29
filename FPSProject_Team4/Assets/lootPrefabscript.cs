using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lootPrefabscript : MonoBehaviour
{
    [SerializeField] public Loot loot;
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log(collision);
            Debug.Log(loot);
            Destroy(gameObject); //TO DO: add loot pick ups to player inventory
        }
    }
}
