using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemMovement : MonoBehaviour
{
    private void Start()
    {
        transform.DOMoveY(transform.position.y + 0.5f,1f).SetLoops(-1,LoopType.Yoyo);
    }
}
