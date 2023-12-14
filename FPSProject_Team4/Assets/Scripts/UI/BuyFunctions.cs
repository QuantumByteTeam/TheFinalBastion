using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyFunctions : MonoBehaviour
{
    [Range(0, 1)][SerializeField] float healModifier;
    [SerializeField] float speedModifer;
    [SerializeField] float sprintModifer;
    //[SerializeField] int armorModifer;
    [SerializeField] float maxHPModifer;

    public void buyHeal()
    {
        PlayerController player = GameManager.instance.playerScript;
        if (player.HP < player.HPOriginal)
        {
            player.HP = player.HPOriginal * healModifier;
            UIManager.instance.UpdatePlayerHP();
        }
    }

    public void buyArmor()
    {
        GameManager.instance.playerScript.armor = true;
    }

    public void buySpeed()
    {
        GameManager.instance.playerScript.PlayerSpeed *= speedModifer;
    }

    public void buyMaxHP()
    {
        GameManager.instance.playerScript.HPOriginal *= maxHPModifer;
        UIManager.instance.UpdatePlayerHP();
    }

    public void buyAmmo()
    {
        List<gunStats> guns = GameManager.instance.playerScript.gunList;
        for (int i = 0; i < guns.Count; i++)
        {
            guns[i].ammoReserve = guns[i].ammoReserveDefault;
        }

        UIManager.instance.UpdateAmmo();
    }
}
