using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject InventoryObject;
    
    public void OpenInventory()
    {
        UpdateInventory();
        InventoryObject.gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        InventoryObject.gameObject.SetActive(false);
    }

    public void UpdateInventory()
    {
        foreach (var item in InventoryGameUpdate.instance.objects)
        {
            item.SetInventoryItem();
            item.SetItemImage();
        }
    }
    
}
