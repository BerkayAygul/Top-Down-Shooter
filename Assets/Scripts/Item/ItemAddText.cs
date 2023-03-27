using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ItemAddText : MonoBehaviour
{
    TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        gameObject.transform.DOMoveY(transform.position.y + 2, 1.95f);
        text.DOFade(0, 2).OnComplete(() => Destroy(gameObject));
    }
}
