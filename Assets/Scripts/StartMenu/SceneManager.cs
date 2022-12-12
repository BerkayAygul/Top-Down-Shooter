using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
  public void CharacterScreen()
  {
    UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterMenu");
  }
  public void Options()
  {
    UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
  }
  public void CoopScreen()
  {
    UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu");
  }
  public void Exit()
  {
    Application.Quit();
  }
}
