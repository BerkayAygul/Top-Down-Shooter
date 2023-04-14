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

  public ClassScriptable saveClassScriptable;

  private void Start()
  {
    currentClassImage.sprite = classImages[0];
    currentClassName.text = selectedClass.ToString();
  }

  public void GetRigthClass()
  {
    if (selectedClass == PlayerData.Classes.warrior)
    {
      currentClassImage.sprite = classImages[0];
      currentClassName.text = selectedClass.ToString();
    }
    else if (selectedClass == PlayerData.Classes.mage)
    {
      currentClassImage.sprite = classImages[1];
      currentClassName.text = selectedClass.ToString();
    }
    else if (selectedClass == PlayerData.Classes.archer)
    {
      currentClassImage.sprite = classImages[2];
      currentClassName.text = selectedClass.ToString();
    }
    
  }

  public void NextClass()
  {
    if (selectedClass == PlayerData.Classes.archer )
    {
      selectedClass = PlayerData.Classes.warrior;
    }
    else
    {
      selectedClass++;
    }
    GetRigthClass();
  }

  public void Continue()
  {
    saveClassScriptable.currentClass = selectedClass;
    UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu");
  }
  public void BackClass()
  {
    if (selectedClass <= PlayerData.Classes.warrior )
    {
      selectedClass = PlayerData.Classes.archer;
    }
    else
    {
      selectedClass--;
    }
    GetRigthClass();
  }
  
}
