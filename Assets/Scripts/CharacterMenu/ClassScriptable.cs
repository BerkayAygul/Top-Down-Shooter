using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassScriptable:MonoBehaviour
{
    public static ClassScriptable instance;
    public PlayerData.Classes currentClass;

    public ClassScriptable()
    {
        instance = this;
    }

    private void Start()
    {
        currentClass = PlayerData.Classes.gunner;
    }
}
