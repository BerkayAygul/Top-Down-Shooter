using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
   private int currentPlayerActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
   public GameObject panel;
   public GameObject itemPrefab;
   TextMeshProUGUI itemText;
   
   
   public void ShowItems()
   {
       if (panel.activeInHierarchy)
       {
           foreach (Transform child in panel.transform) { //
               Destroy(child.gameObject);
           }
           panel.SetActive(false);
       }
       else
       {
           panel.SetActive(true);
           foreach (var item in MatchManager.instance.inventories[currentPlayerActorNum].items)
           {
               GameObject itemImage = Instantiate(itemPrefab,panel.transform);
               itemText = itemImage.GetComponentInChildren<TextMeshProUGUI>();
               itemImage.transform.SetParent(panel.transform);
               itemText.text = item.itemName;
           }
       }
       
   }
}
