using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpriteSet : MonoBehaviour
{
   [SerializeField]public Item item;
   public SpriteRenderer spriteRenderer;

   private void Start()
   {
      spriteRenderer.sprite = item.itemSprite;
   }
}
