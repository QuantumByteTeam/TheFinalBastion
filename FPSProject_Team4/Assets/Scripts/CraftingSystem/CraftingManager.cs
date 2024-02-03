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

    public Sprite SlotDefault;
    public GameObject PlayerPos;
    
    Vector3 spawnPosition;

    //public playerInventory inventory; //ref player inv for public int access
    int clothCount;
    int bombCount;
    int chipCount;
    int scrapCount;

    /*public GameObject cloth;
    public GameObject scrap;
    public GameObject bomb;
    public GameObject chip;*/


    private void Update()
    {
        //update mat counts

        


        


        Vector3 playerPos = transform.position; //gets player position

        

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
                    //slot.gameObject.SetActive(false);
                    slot.GetComponent<Image>().sprite = SlotDefault;
                }
            }
            //clears result slot when exiting
            resultSlot.item = null;
            resultSlot.gameObject.SetActive(false);
            resultSlot.GetComponent<Image>().sprite = SlotDefault;
            Craftable = false;

        }

    }

    void CheckForRecipe(params CraftingSlot[] slots)
    {
        nonNullSlotsCount = 0;
        //Craftable = false; 1/26 commented out
        resultSlot.item = null;
        resultSlot.gameObject.SetActive(false);
        resultSlot.GetComponent<Image>().sprite = SlotDefault;


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
        slot.gameObject.GetComponent<Image>().sprite = SlotDefault;
        CheckForRecipe();
    }

    public void OnConfirmRecipe() 
    {
        clothCount = GameManager.instance.playerScript.inventory.Cloth;
        bombCount = GameManager.instance.playerScript.inventory.Exp;
        chipCount = GameManager.instance.playerScript.inventory.Circuits;
        scrapCount = GameManager.instance.playerScript.inventory.Scrap;
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

            //Vector3 playerPosition = transform.position; //gets player position
            //Debug.Log("Player Pos: " + PlayerPos); //logs it into debug
            //Vector3 spawnPosition = PlayerPos;
            //Vector3 spawnPosition = table.SpawnPos(); //sets spawn position for craftable item

            Vector3 playerPos = PlayerPos.transform.position;
            Vector3 spawnPosition = PlayerPos.transform.position;

            


            if (currRecipeString == "ScrapCompChipCompChipnull" && scrapCount >= 1 && chipCount >= 2) //Turret item get
            {
                GameObject CratableSpawn = Instantiate(TurretPrefab, spawnPosition, Quaternion.identity); //spawns item inside player (auto pickup)
                GameManager.instance.playerScript.inventory.Scrap = GameManager.instance.playerScript.inventory.Scrap - 1; //updates count
                GameManager.instance.playerScript.inventory.Circuits = GameManager.instance.playerScript.inventory.Circuits - 2;
                UIManager.instance.updateMats();
            }
            else //cannot craft not enough components
            {
                
            }
            if (currRecipeString == "ScrapScrapnullnull" && scrapCount >= 2) //Barbed Wire item get
            {
                GameObject CratableSpawn = Instantiate(BarbedWirePrefab, spawnPosition, Quaternion.identity); //spawns item inside player (auto pickup)
                GameManager.instance.playerScript.inventory.Scrap = GameManager.instance.playerScript.inventory.Scrap - 2; //updates count
                UIManager.instance.updateMats();
            }
            else //cannot craft not enough components
            {

            }
            if (currRecipeString == "ScrapBombBombCompChip" && scrapCount >= 1 && bombCount >= 2 && chipCount >= 1) //Mine item get
            {
                GameObject CratableSpawn = Instantiate(MinePrefab, spawnPosition, Quaternion.identity); //spawns item inside player (auto pickup)
                GameManager.instance.playerScript.inventory.Scrap = GameManager.instance.playerScript.inventory.Scrap - 1; //updates count
                GameManager.instance.playerScript.inventory.Scrap = GameManager.instance.playerScript.inventory.Exp - 2; 
                GameManager.instance.playerScript.inventory.Scrap = GameManager.instance.playerScript.inventory.Circuits - 1;
                UIManager.instance.updateMats();
            }
            else //cannot craft not enough components
            {

            }












            // Clear the crafting slots and reset the UI/item indexes
            resultSlot.item = null;
                resultSlot.gameObject.SetActive(false);
                resultSlot.GetComponent<Image>().sprite = SlotDefault;
                foreach (CraftingSlot slot in craftingSlots) //clears all crafting slots
                {
                    if (slot != null)
                    {
                        slot.item = null;
                        //slot.gameObject.SetActive(false);
                        slot.GetComponent<Image>().sprite = SlotDefault;
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
