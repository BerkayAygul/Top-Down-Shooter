using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "Item",menuName = "ScriptableObjects/Item",order = 1)]
public class ItemScriptable : ScriptableObject
{
    //Item Properties

    public string itemName;
    public Sprite itemSprite;
    public Image itemImage;
    public int itemLevel;
    public float itemDropChance;
    public RequiredClass requiredClass;
    public ItemRarity itemRarity;
    public ItemType itemType;

    
    //Stats
    public float strenght;
    public float vitality;
    public float agility;
    public float intelligent;

    public enum RequiredClass
    {
        Mage,
        Warrior,
        Archer
    }
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mystic
    }
    public enum ItemType
    {
        MainWeapon,
        SecondaryWeapon,
        Armor,
        Boots
    }
}

