using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class hotbar : MonoBehaviour
{
    [SerializeField] Image hotbarImage;
    [SerializeField] Image hotbarSelection;
    [SerializeField] GameObject player;

    inventoryItem[] hotbarInventory;
    inventoryItem[] craftingInventory;
}