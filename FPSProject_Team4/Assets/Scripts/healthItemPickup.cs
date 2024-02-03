using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class healthItemPickup : MonoBehaviour
{

    bool triggerSet; //code that fixes a bug with picking up multiple of the same type of gun (activates trigger multiple times)
    [SerializeField] float amount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerSet)
        {
            triggerSet = true;
            //give stats to player

            GameManager.instance.playerScript.HP += amount;

            UIManager.instance.UpdatePlayerHP();

            Destroy(gameObject);
            
        }


    }


}