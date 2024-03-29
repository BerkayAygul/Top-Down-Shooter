using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassController : MonoBehaviour
{
  public Sprite[] classImages;
  public PlayerData.Classes selectedClass;

  public Image currentClassImage;
  public TextMeshProUGUI currentClassName;

  public GameObject choosePanel;

  private void Start()
  {
    selectedClass = PlayerData.Classes.gunner;
    GetRigthClass();
  }

  public void GetRigthClass()
  {
    if (selectedClass == PlayerData.Classes.gunner)
    {
      currentClassImage.sprite = classImages[0];
      currentClassName.text = selectedClass.ToString().ToUpper();
    }
    else if (selectedClass == PlayerData.Classes.commando)
    {
      currentClassImage.sprite = classImages[1];
      currentClassName.text = selectedClass.ToString().ToUpper();
    }
    ClassScriptable.instance.currentClass = selectedClass;
  }

  public void NextClass()
  {
    if (selectedClass == PlayerData.Classes.gunner )
    {
      selectedClass = PlayerData.Classes.commando;
    }
    else if (selectedClass == PlayerData.Classes.commando)
    {
      selectedClass = PlayerData.Classes.gunner;
    }
    GetRigthClass();
  }

  public void Continue()
  {
    choosePanel.SetActive(false);
  }
  public void BackClass()
  {
    if (selectedClass == PlayerData.Classes.gunner )
    {
      selectedClass = PlayerData.Classes.commando;
    }
    else if (selectedClass == PlayerData.Classes.commando)
    {
      selectedClass = PlayerData.Classes.gunner;
    }
    GetRigthClass();
  }
  
}
