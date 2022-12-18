using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetItemText : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private TextMeshProUGUI itemNameText;

    private void Start()
    {
        CheckRarity(item);
        itemNameText.text = item.itemName;
    }

    private void CheckRarity(Item item)
    {
        switch (item.itemRarity)
        {
            case ItemScriptable.ItemRarity.Uncommon:
                itemNameText.color = Color.white;
                return;
            case ItemScriptable.ItemRarity.Common:
                itemNameText.color = Color.green;
                return;
            case ItemScriptable.ItemRarity.Rare:
                itemNameText.color = Color.blue;
                return;
            case ItemScriptable.ItemRarity.Epic:
                itemNameText.color = Color.magenta;
                return;
            case ItemScriptable.ItemRarity.Legendary:
                itemNameText.color = Color.yellow;
                return;
            case ItemScriptable.ItemRarity.Mystic:
                itemNameText.color = new Color(133, 0, 0);
                return;
        }
    }
}
