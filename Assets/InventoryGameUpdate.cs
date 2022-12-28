using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGameUpdate : MonoBehaviour
{
    public static InventoryGameUpdate instance;
    public List<InventoryController> objects;

    private void Awake()
    {
        instance = this;
    }
}
