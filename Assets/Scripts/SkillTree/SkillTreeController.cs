using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeController : MonoBehaviour
{
    public static SkillTreeController instance;
    [SerializeField] private GameObject skillTreePanel;
    private GameObject player;
    private PlayerAttributes playerAttributes;
    public TextMeshProUGUI dexText, strText, vitText, intText;
    public TextMeshProUGUI remainStatText;
    public TextMeshProUGUI currentExpText,maxExpText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI classText;
    public Image expBarImage;

    public SkillTreeController()
    {
        instance = this;
    }

    private void Start()
    {
        GetPlayer();
       
        Debug.Log("Girdi"); 
        playerAttributes = player.GetComponent<PlayerAttributes>();
        UpdateText();
        UpdateExpValuesOnText();
        
    }

    private void GetPlayer()
    {
        foreach (var playerInList in MatchManager.instance.playersGameObjects)
        {
            if (playerInList.GetComponent<PhotonView>().ControllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                player = playerInList;
                
            }
        }
    }

    private void UpdateText()
    {
        dexText.text = playerAttributes.dexterity.ToString();
        vitText.text = playerAttributes.vitality.ToString();
        intText.text = playerAttributes.intelligence.ToString();
        strText.text = playerAttributes.strength.ToString();
        remainStatText.text = playerAttributes.statPoints.ToString();
        classText.text = playerAttributes.playerClass.ToString();

    }
    public void UpdateExpValuesOnText()
    {
        currentExpText.text = playerAttributes.playerCurrentExperience.ToString();
        maxExpText.text = playerAttributes.playerMaxExperience.ToString();
        expBarImage.fillAmount =
            Mathf.Repeat((float)playerAttributes.playerCurrentExperience / (float)playerAttributes.playerMaxExperience,
                1.0f);
        
        remainStatText.text = playerAttributes.statPoints.ToString();
        levelText.text = playerAttributes.playerLevel.ToString();
    }

    public void OpenSkillTree()
    {
        if (!skillTreePanel.activeInHierarchy)
        {
            
            skillTreePanel.SetActive(true);
            UpdateText();
        }
        else
        {
            CloseSkillTree();
        }
    }
    public void CloseSkillTree()
    {
        if (skillTreePanel.activeInHierarchy)
        {
            skillTreePanel.SetActive(false);
        }
        
    }
    //Save stats
    public void SaveStats()
    {
        playerAttributes.SavePlayer();
    }

    public void ResetStats()
    {
        playerAttributes.intelligence = 1;
        playerAttributes.dexterity = 1;
        playerAttributes.strength = 1;
        playerAttributes.vitality = 1;
        playerAttributes.statPoints = playerAttributes.playerLevel;
        UpdateText();
    }

    //stat increase section
    public void IncreaseDex()
    {
        if (playerAttributes.statPoints > 1)
        {
            playerAttributes.statPoints--;
            playerAttributes.dexterity++;
            UpdateText();
        }
    }
    public void IncreaseInt()
    {
        if (playerAttributes.statPoints > 0)
        {
            playerAttributes.statPoints--;
            playerAttributes.intelligence++;
            UpdateText();
        }
    }
    public void IncreaseStr()
    {
        if (playerAttributes.statPoints > 0)
        {
            playerAttributes.statPoints--;
            playerAttributes.strength++;
            UpdateText();
        }
    }
    public void IncreaseVit()
    {
        if (playerAttributes.statPoints > 0)
        {
            playerAttributes.statPoints--;
            playerAttributes.vitality++;
            UpdateText();
        }
    }
    //stat decreasing section
    public void DecreaseDex()
    {
        if (playerAttributes.statPoints <= playerAttributes.playerLevel)
        {
            
            if (playerAttributes.dexterity > 0)
            {
                playerAttributes.dexterity--;
                playerAttributes.statPoints++;
            }
            
            UpdateText();
        }
    }
    public void DecreaseInt()
    {
        if (playerAttributes.statPoints <= playerAttributes.playerLevel)
        {
            
            if (playerAttributes.intelligence > 0)
            {
                playerAttributes.intelligence--;
                playerAttributes.statPoints++;
            }
            
            UpdateText();
        }
    }
    public void DecreaseStr()
    {
        if (playerAttributes.statPoints <= playerAttributes.playerLevel)
        {
            
            if (playerAttributes.strength > 0)
            {
                playerAttributes.strength--;
                playerAttributes.statPoints++;
            }
            
            UpdateText();
        }
    }
    public void DecreaseVit()
    {
        if (playerAttributes.statPoints <= playerAttributes.playerLevel)
        {
            
            if (playerAttributes.vitality > 0)
            {
                playerAttributes.vitality--;
                playerAttributes.statPoints++;
            }
            
            UpdateText();
        }
    }
}
