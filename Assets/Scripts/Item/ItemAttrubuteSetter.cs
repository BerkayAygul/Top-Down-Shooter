using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttrubuteSetter : MonoBehaviour
{
    public ItemScriptable itemScriptable;
    public Item item;
    private void Awake()
    {
        item.agility = itemScriptable.agility;
        item.strenght = itemScriptable.strenght;
        item.intelligent = itemScriptable.intelligent;
        item.vitality = itemScriptable.vitality;
        item.itemName = itemScriptable.itemName;
        item.itemRarity = itemScriptable.itemRarity;
        item.itemType = itemScriptable.itemType;
        item.itemImage = itemScriptable.itemImage;
        item.itemSprite = itemScriptable.itemSprite;
        item.itemLevel = itemScriptable.itemLevel;
        item.requiredClass = itemScriptable.requiredClass;
    }
}
