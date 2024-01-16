using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBool : MonoBehaviour
{
    public bool HasWrench = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) //DEV TOOL TEMP! press q to toggle if player has a wrench or not
        {
            HasWrench = !HasWrench;
        }
    }
}
