using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used to contain and call data pertaining to the snowball item tiers
public class SnowballItemTiers : MonoBehaviour
{
    public static SnowballItemTiers Instance = null;
    public SnowballItemsClass[] itemLines;
    public GameObject rubberDucky;

    enum Chance_Effects
    {
        UPGRADE = 0,
        CHANGE_LINE,
        DOWNGRADE,
        DOUBLE_UPGRADE,
        DUCKY
    }
    
    // Visible values used for determining chances of getting a certain outcome
    public int[] tierChances =
    {
        50,     // Upgrade
        35,     // Different item, same tier
        5,      // Downgrade
        5,      // Double upgrade
        5       // Produce ducky
    };

    // Values that conglomerate the above into a 0 to 100 scale
    public int[] actualTierChances;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start ()
    {
        // Sets each line's ID number for easy use
		for (int i = 0; i < itemLines.Length; i++)
        {
            itemLines[i].SetIDNumber(i);
        }

        // Sets up the "ranges" for each of the checked values
        actualTierChances = new int[tierChances.Length];
        for (int i = 0; i < actualTierChances.Length; i++)
        {
            actualTierChances[i] = 0;
        
            if (tierChances[i] > 0)
            {
                for (int j = i; j >= 0; j--)
                {
                    actualTierChances[i] += tierChances[j];
                }
            }
        }
	}

    // placementNum = the number each item falls in a select tier; may need to be set up in prefabs (or checked with each object?)
    // lineID       = the ID value of each object's "line" type (i.e., rocks vs. knives)
    public GameObject CheckNewObject(int lineID, int placementNum, int containedItems)
    {
        GameObject temp = null;

        int checkingRange = Random.Range(0, 100);

        // --- IF SNOWBALL IS EMPTY ---
        if (containedItems == 0)
        {
            // Grabs a random "Tier 1" item
            checkingRange = Random.Range(0, itemLines.Length);
            temp = itemLines[checkingRange].items[0];
            temp.GetComponent<ItemDataScript>().SetValues(checkingRange, 0);

            // Returns it
            return temp;
        }

        // --- IF SNOWBALL CONTAINS MORE THAN ONE ITEM ---
        if (containedItems > 1)
        {
            Debug.Log("CANNOT RETURN");
            return null;
        }

        // --- IF SNOWBALL CONTAINS AN ITEM ---
        Debug.Log(checkingRange + ", " + actualTierChances[(int)Chance_Effects.UPGRADE]);

        // Checks to increase an item's "tier"
        if (checkingRange < actualTierChances[(int)Chance_Effects.UPGRADE])
        {
            // Produces an upgraded item if the player does not have the highest item in the line
            if ((itemLines[lineID].items.Length - 1) >= (placementNum + 1))
            {
                temp = itemLines[lineID].items[placementNum + 1];
                temp.GetComponent<ItemDataScript>().SetValues(lineID, placementNum + 1);
            }

            // Produces the same item again if the player has the highest tier in the current line
            else
            {
                temp = itemLines[lineID].items[placementNum];
                temp.GetComponent<ItemDataScript>().SetValues(lineID, placementNum);
            }
        }

        // Changes an item to one from a different line that contains the same
        else if (checkingRange >= actualTierChances[(int)Chance_Effects.UPGRADE] && checkingRange < actualTierChances[(int)Chance_Effects.CHANGE_LINE])
        {
            // Repurposes checkingRange, since it's technically irrelevant now
            checkingRange = Random.Range(0, itemLines.Length);

            // Checks if the selected tier has an item that matches the current placement number OR if the new line does not match the current one
            // If not, choose a different tier, instead
            while ((itemLines[checkingRange].items.Length - 1) < placementNum || checkingRange == lineID)
            {
                checkingRange = Random.Range(0, itemLines.Length);
            }
            
            temp = itemLines[checkingRange].items[placementNum];
            temp.GetComponent<ItemDataScript>().SetValues(checkingRange, placementNum);
        }
        
        // Downgrades the current item, if applicable
        else if (checkingRange >= actualTierChances[(int)Chance_Effects.CHANGE_LINE] && checkingRange < actualTierChances[(int)Chance_Effects.DOWNGRADE])
        {
            // If the current item is above Tier 1, downgrade it
            if (placementNum > 0)
            {
                temp = itemLines[lineID].items[placementNum - 1];
                temp.GetComponent<ItemDataScript>().SetValues(lineID, placementNum - 1);
            }
        
            // If the current tier item is below Tier 1, keep it the same
            // Possibly rerun the script, instead?
            else
            {
                checkingRange = Random.Range(0, itemLines.Length);
                temp = itemLines[checkingRange].items[0];
                temp.GetComponent<ItemDataScript>().SetValues(lineID, placementNum);
            }
        }
        
        // Upgrades the current line's item twice, if applicable
        else if (checkingRange >= actualTierChances[(int)Chance_Effects.DOWNGRADE] && checkingRange < actualTierChances[(int)Chance_Effects.DOUBLE_UPGRADE])
        {
            // Produces an upgraded item if the player does not have at least the second-highest item in the line
            if ((itemLines[lineID].items.Length - 1) >= (placementNum + 2))
            {
                temp = itemLines[lineID].items[placementNum + 2];
                temp.GetComponent<ItemDataScript>().SetValues(lineID, placementNum + 2);
            }
        
            // Produces an upgraded item if the player does not have the highest item in the line
            else if ((itemLines[lineID].items.Length - 1) >= (placementNum + 1))
            {
                temp = itemLines[lineID].items[placementNum + 1];
                temp.GetComponent<ItemDataScript>().SetValues(lineID, placementNum + 1);
            }
        
            // Produces the same item again if the player has the highest tier in the current line
            else
            {
                temp = itemLines[lineID].items[placementNum];
                temp.GetComponent<ItemDataScript>().SetValues(lineID, placementNum);
            }
        }

        // Ejects a fun rubber ducky to play with!
        else if (checkingRange >= actualTierChances[(int)Chance_Effects.DOUBLE_UPGRADE] && checkingRange < actualTierChances[(int)Chance_Effects.DUCKY])
        {
            temp = rubberDucky;
        }

        // Returns the new GameObject
        return temp;
    }
}
