using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    
    private CraftingItem currItem; //stores the current item
    public Image customCursor;

    public CraftingSlot[] craftingSlots;

    public List<CraftingItem> itemList;
    public string[] recipes;
    public CraftingItem[] recipeResults;
    public CraftingSlot resultSlot;
    bool Craftable = false;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (currItem != null)
            {
                customCursor.gameObject.SetActive(false);
                CraftingSlot nearestSlot = null;
                float closestDist = float.MaxValue;

                foreach(CraftingSlot slot in craftingSlots)
                {
                    float dist = Vector2.Distance(Input.mousePosition, slot.transform.position);

                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        nearestSlot = slot;
                    }
                }
                nearestSlot.gameObject.SetActive(true);
                nearestSlot.GetComponent<Image>().sprite = currItem.GetComponent<Image>().sprite;
                nearestSlot.item = currItem;
                itemList[nearestSlot.index] = currItem;

                currItem = null;

                CheckForRecipe();
            }
        }


    }

    void CheckForRecipe()
    {

        Craftable = false;
        resultSlot.gameObject.SetActive(false); 
        resultSlot.item = null;

        string currRecipeString = "";
        foreach(CraftingItem item in itemList)
        {
            if (item != null)
            {
                currRecipeString += item.itemName;
            }
            else
            {
                currRecipeString += "null";
            }
        }

        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipes[i] == currRecipeString)
            {
                resultSlot.gameObject.SetActive(true);
                resultSlot.GetComponent<Image>().sprite = recipeResults[i].GetComponent<Image>().sprite;
                resultSlot.item = recipeResults[i];
                Craftable = true;
            }
            else
            {
                Craftable = false;
            }
        }



    }


    public void OnClickSlot(CraftingSlot slot)
    {
        slot.item = null;
        itemList[slot.index] = null;
        slot.gameObject.SetActive(false);
        CheckForRecipe();
    }

    public void OnConfirmRecipe(CraftingSlot slot)
    {
        if (Craftable == true)
        {
            //slot.gameObject.SetActive(true);
            //resultSlot.item = list of some sort              ( code for obtaining the item goes here )

            //lines below clear the item from the result slot
            slot.item = null;
            slot.gameObject.SetActive(false);

        }
    }


    public void OuMouseDownItem(CraftingItem item) //if current item is held down by Left click then it will change the cursor to the item that it was hovering over
    {
        if (currItem == null)
        {
            currItem = item;
            customCursor.gameObject.SetActive(true); 
            customCursor.sprite = currItem.GetComponent<Image>().sprite;
        }




    }
}
