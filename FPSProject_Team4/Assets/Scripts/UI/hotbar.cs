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

    [SerializeField] TMP_Text clothAmt;
    [SerializeField] TMP_Text metalAmt;
    [SerializeField] TMP_Text expAmt;
    [SerializeField] TMP_Text circuitsAmt;

    //inventoryItem[] hotbarInventory;

    [SerializeField] GameObject cloth;
    [SerializeField] GameObject metal;
    [SerializeField] GameObject explosives;
    [SerializeField] GameObject circuits;

    private int selection; //0-7
}