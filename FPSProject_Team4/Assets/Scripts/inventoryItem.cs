using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Inventory Item")]
public class inventoryItem : ScriptableObject
{
    public string itemID;
    [SerializeField] Image icon;
}