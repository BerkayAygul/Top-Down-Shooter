using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory",menuName = "ScriptableObjects/Inventory",order = 2)]
public class InventoryScriptable : ScriptableObject
{
  public List<InventoryItem> items;

  public void AddItem(InventoryItem item, int count)
  {
    item.itemCount = count;
    items.Add(item);
  }
}
