using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    private InventoryItem inventoryItem;
    public GameObject inventory;
    private int count = 1;

    private void Start()
    {
        inventoryItem = new InventoryItem();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (gameObject.transform.GetComponent<Item>() != null)
            {
                CreateItemForInventory(inventoryItem);
            }

            if (col.GetComponent<InventoryController>().inventory.items.Count <= 21)
            {
                col.GetComponent<InventoryController>().inventory.AddItem(inventoryItem,count);
            }
            else
            {
                Debug.Log("Not enough space in inventory!!");
            }
            Destroy(gameObject);
        }
    }

    private void CreateItemForInventory(InventoryItem item)
    {
        
        Item copiedItem = gameObject.transform.GetComponent<Item>();
        Debug.Log("copiedItem.agility = " + copiedItem.agility);
        item.agility = copiedItem.agility;
        item.intelligent = copiedItem.intelligent;
        item.strenght = copiedItem.strenght;
        item.vitality = copiedItem.vitality;
        item.itemImage = copiedItem.itemImage;
        item.itemLevel = copiedItem.itemLevel;
        item.itemName = copiedItem.itemName;
        item.itemRarity = copiedItem.itemRarity;
        item.itemSprite = copiedItem.itemSprite;
        item.itemType = copiedItem.itemType;
        item.requiredClass = copiedItem.requiredClass;
        item.itemDropChance = copiedItem.itemDropChance;
    }
}
