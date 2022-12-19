using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDrop : MonoBehaviour
{
   public List<GameObject> itemList;
   private void OnDestroy()
   {
      DropItem();
   }

  

   private GameObject GetDropItem()
   {
      int randomNumber = Random.Range(1, 101);
      List<GameObject> possibleItems = new List<GameObject>();
      foreach (GameObject item in itemList)
      {
         if (randomNumber <= item.GetComponent<Item>().itemDropChance)
         {
            possibleItems.Add(item);
         }
      }

      if (possibleItems.Count > 0)
      {
         GameObject droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
         return droppedItem;
      }
      return null;
   }

   private void DropItem()
   {
      GameObject droppedItem = GetDropItem();
      if (droppedItem != null)
      {
         Instantiate(droppedItem);
      }
   }
}
