using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;
public class Item
{
    //Item Properties
    public string itemName;
    public Sprite itemSprite;
    public Image itemImage;
    public int itemLevel;
    public float itemDropChance;
    public ItemScriptable.RequiredClass requiredClass;
    public ItemScriptable.ItemRarity itemRarity;
    public ItemScriptable.ItemType itemType;

    //Stats
    public float strenght;
    public float vitality;
    public float agility;
    public float intelligent;
}
