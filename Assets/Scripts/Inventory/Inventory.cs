using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Inventory : MonoBehaviour
{
   public List<Item> items = new List<Item>();

   public int ownerActorNum {
       get
       {
           return gameObject.GetComponent<PhotonView>().ControllerActorNr;
       }
   }
}

