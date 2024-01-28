using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    
    private CraftingItem currItem; //stores the current item   
    public Image customCursor;

    public CraftingSlot[] craftingSlots;                   //make sure to drag and drop crafting manager into UI since its still buggy when its actually in UI


    public List<CraftingItem> itemList;
    public string[] recipes;
    public CraftingItem[] recipeResults;
    public CraftingSlot resultSlot;
    bool Craftable = false;
    public string currRecipeString;
    public int nonNullSlotsCount;

    public GameObject AmmoPrefab;
    public GameObject TurretPrefab;
    public GameObject BarbedWirePrefab;
    public GameObject MinePrefab;
    public GameObject ArmorPrefab;
    public GameObject BandagePrefab;

    



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
        resultSlot.item = null;
        resultSlot.gameObject.SetActive(false);
        resultSlot.GetComponent<Image>().sprite = null;


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

        Debug.Log("nonNullSlotsCount: " + nonNullSlotsCount);
        Debug.Log("currRecipeString: " + currRecipeString);

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
            if (Craftable == true && nonNullSlotsCount >= 2)
            {
            // Handle the crafted item (obtain and use it)

            Vector3 playerPosition = transform.position; //gets player positionm
            Debug.Log("Player Pos: " + playerPosition); //logs it into debug
            Vector3 spawnPosition = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z); //sets spawn position for craftable item

            if (currRecipeString == "ScrapCompChipCompChipnull") //Turret item get
            {
                GameObject CratableSpawn = Instantiate(TurretPrefab, spawnPosition, Quaternion.identity); //spawns item inside player (auto pickup)
            }
            if (currRecipeString == "ScrapScrapnullnull") //Barbed Wire item get
            {
                GameObject CratableSpawn = Instantiate(BarbedWirePrefab, spawnPosition, Quaternion.identity); //spawns item inside player (auto pickup)
            }
            if (currRecipeString == "ScrapBombBombCompChip") //Mine item get
            {
                GameObject CratableSpawn = Instantiate(MinePrefab, spawnPosition, Quaternion.identity); //spawns item inside player (auto pickup)
            }












            // Clear the crafting slots and reset the UI/item indexes
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
                currRecipeString = "";
                //itemList[resultSlot.index] = null;
                nonNullSlotsCount= 0;
                currItem= null;
                //Debug.Log("nonNullSlotsCount: " + nonNullSlotsCount);
                //Debug.Log("currRecipeString: " + currRecipeString);

                for (int i = 0; i < 4; i++)
            {
                itemList[i] = null; //essential for the items to not craft at half cost
            }

                //itemList[2] = null; 
                //itemList[1] = null; 
        }
        
    }

    public void OnCraftButtonClick() //use this so that u can use onClick buttons easily
    {
        OnConfirmRecipe();
        //nonNullSlotsCount = 0;
        
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

    void GetPlayerPos()
    {
        Vector3 playerPosition = transform.position;
        Debug.Log("Player Pos: " + playerPosition);
    }

}
