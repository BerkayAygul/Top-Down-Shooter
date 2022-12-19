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
            case ItemScriptable.ItemRarity.Common:
                itemNameText.color = new Color32(255, 255, 208,255);
                return;
            case ItemScriptable.ItemRarity.Uncommon:
                itemNameText.color = new Color32(130, 205, 71,255);
                return;
            case ItemScriptable.ItemRarity.Rare:
                itemNameText.color = new Color32(33, 70, 199,255);
                return;
            case ItemScriptable.ItemRarity.Epic:
                itemNameText.color = new Color32(165, 85, 236,255);
                return;
            case ItemScriptable.ItemRarity.Legendary:
                itemNameText.color = new Color32(255, 174, 109,255);
                return;
            case ItemScriptable.ItemRarity.Mystic:
                itemNameText.color = new Color32(133, 0, 0,255);
                return;
        }
    }
}
