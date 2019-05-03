using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataScript : MonoBehaviour
{
    // Integers that store the item's "line" and placement therein
    private int lineID, tierNum;

	public void SetValues(int newLineID, int newTierNum)
    {
        lineID = newLineID;
        tierNum = newTierNum;
    }

    public int GetLineID()
    {
        return lineID;
    }

    public int GetTierNum()
    {
        return tierNum;
    }
}
