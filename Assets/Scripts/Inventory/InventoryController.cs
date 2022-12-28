using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
   public InventoryScriptable inventory;
   private InventoryItem inventoryItem;
   public Image itemImage;
   public void SetInventoryItem()
   {
      int index = 0;
      foreach (InventoryItem item in inventory.items)
      {
         if (item != null);
         index++;
         if (index ==  int.Parse(gameObject.name))
         {
            inventoryItem = item;
            break;
         }

      }
   }

   public void SetItemImage()
   {
      if (inventoryItem != null)
      {
         itemImage.gameObject.SetActive(true);
         itemImage.sprite = inventoryItem.itemSprite;
      }
   }

   public void UpdateInventory()
   {
      SetInventoryItem();
      SetItemImage();
   }

 
}
