using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite itemSprite;
    public Image itemImage;
    public int itemLevel;
    public float itemDropChance;
    public ItemScriptable.RequiredClass requiredClass;
    public ItemScriptable.ItemRarity itemRarity;
    public ItemScriptable.ItemType itemType;
    public int itemCount;

    //Stats
    public float strenght;
    public float vitality;
    public float agility;
    public float intelligent;
    
}
