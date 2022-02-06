using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> itemList;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;

    public Inventory() {
        itemList = new List<Item>();

        Debug.Log("Inventory Init");
    }
}
