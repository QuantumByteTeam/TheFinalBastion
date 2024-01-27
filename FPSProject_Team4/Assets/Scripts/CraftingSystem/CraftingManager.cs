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
    public string currRecipeString;
    public int nonNullSlotsCount;

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
        if (Input.GetKeyDown(KeyCode.Q)) //clears the UI when exiting
        {
            foreach (CraftingSlot slot in craftingSlots) //clears all crafting slots
            {
                if (slot != null)
                {
                    slot.item = null;
                    slot.gameObject.SetActive(false);
                    slot.GetComponent<Image>().sprite = null;
                }
            }
            //clears result slot when exiting
            resultSlot.item = null;
            resultSlot.gameObject.SetActive(false);
            resultSlot.GetComponent<Image>().sprite = null;
            Craftable = false;

        }

    }

    void CheckForRecipe(params CraftingSlot[] slots)
    {
        nonNullSlotsCount = 0;
        //Craftable = false; 1/26 commented out
        resultSlot.gameObject.SetActive(false); 
        resultSlot.item = null;

        currRecipeString = "";
        foreach(CraftingItem item in itemList)
        {
            if (item != null)
            {
                nonNullSlotsCount++;
                currRecipeString += item.itemName;
            }
            else
            {
                currRecipeString += "null";
            }
        }

        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipes[i] == currRecipeString && nonNullSlotsCount >= 2) //if recipe is found then show/add item in result slot // <<<<<<< may need to remove this logic bc its like an endless loop and this is likely what causes things to just appear craftable
            {
                resultSlot.gameObject.SetActive(true);
                resultSlot.GetComponent<Image>().sprite = recipeResults[i].GetComponent<Image>().sprite;
                resultSlot.item = recipeResults[i];
                

                
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

    public void OnConfirmRecipe() 
    {
        
        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipes[i] == currRecipeString)
            {
                Craftable = true;
            }
        }
            if (Craftable == true && nonNullSlotsCount >= 2) // didnt work, try setting the crafting string to something that cant ever be crafted to set it back to normal as a fix (null,cloth,null,chip) << also didnt work
            {
            // Handle the crafted item (obtain and use it)

            // Clear the crafting slots
                resultSlot.item = null;
                resultSlot.gameObject.SetActive(false);
                resultSlot.GetComponent<Image>().sprite = null;
                foreach (CraftingSlot slot in craftingSlots) //clears all crafting slots
                {
                    if (slot != null)
                    {
                        slot.item = null;
                        slot.gameObject.SetActive(false);
                        slot.GetComponent<Image>().sprite = null;
                    }
                }

                //Reset Craftable flag
                Craftable = false;
                currRecipeString = "nullClothBombCompChip";
                
            }
        
    }

    public void OnCraftButtonClick() //use this so that u can use onClick buttons easily
    {
        OnConfirmRecipe();
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
