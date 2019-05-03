using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SnowballItemsClass
{
    // The line's ID number (set automatically)
    private int IDNumber = 0;

    // The name used for a given line
    public string lineName;

    // The items contained in a given line
    public GameObject[] items;

    public void SetIDNumber(int num)
    {
        IDNumber = num;
    }
}
