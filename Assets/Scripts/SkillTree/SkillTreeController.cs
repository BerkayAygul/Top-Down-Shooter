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
    
    //Upgrade Section
    [SerializeField] private GameObject attackUpButton;
    [SerializeField] private GameObject hpUpButton;
    [SerializeField] private GameObject speedUpButton;
    [SerializeField] private GameObject specialUpButton;

    [SerializeField] private TextMeshProUGUI attackLevelText;
    [SerializeField] private TextMeshProUGUI speedLevelText;
    [SerializeField] private TextMeshProUGUI hpLevelText;
    [SerializeField] private TextMeshProUGUI specialLevelText;
    public Image expBarImage;
    public GameObject skillChoosePanel;

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

    public void GetPlayer()
    {
        foreach (var playerInList in MatchManager.instance.playersGameObjects)
        {
            if (playerInList.GetComponent<PhotonView>().ControllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                player = playerInList;
                
            }
        }
    }
    public GameObject GetPlayerObject()
    {
        foreach (var playerInList in MatchManager.instance.playersGameObjects)
        {
            if (playerInList.GetComponent<PhotonView>().ControllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return playerInList;

            }
        }

        return null;
    }

    private void UpdateText()
    {
        // dexText.text = playerAttributes.speedLevel.ToString();
        // vitText.text = playerAttributes.vitality.ToString();
        // intText.text = playerAttributes.intelligence.ToString();
        // strText.text = playerAttributes.damage.ToString();
        // remainStatText.text = playerAttributes.statPoints.ToString();
        classText.text = playerAttributes.playerClass.ToString();
        hpLevelText.text = "Level:" + playerAttributes.vitality.ToString();
        specialLevelText.text = "Level:" + playerAttributes.specialLevel.ToString();
        speedLevelText.text = "Level:" + playerAttributes.speedLevel.ToString();
        attackLevelText.text = "Level:" + playerAttributes.damageLevel.ToString();
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
    public void OpenSkillChoosePanel()
    {
        skillTreePanel.SetActive(true);
        skillChoosePanel.SetActive(true);
    }
    public void CloseSkillChoosePanel()
    {
        skillChoosePanel.SetActive(false);
        CloseSkillTree();
    }
    //Save stats
    public void SaveStats()
    {
        playerAttributes.SavePlayer();
    }

    public void ResetStats()
    {
        playerAttributes.intelligence = 1;
        playerAttributes.speedLevel = 1;
        playerAttributes.damage = 30;
        playerAttributes.vitality = 1;
        playerAttributes.statPoints = playerAttributes.playerLevel;
        UpdateText();
    }

    //stat increase section
    public void IncreaseInt()
    {
        if (playerAttributes.statPoints > 0)
        {
            playerAttributes.statPoints--;
            playerAttributes.intelligence++;
            UpdateText();
        }
    }
    public void IncreaseDamage()
    {
        if (playerAttributes.leftSkillPoint > 0)
        {
            playerAttributes.leftSkillPoint--;
            playerAttributes.damageLevel++;
            playerAttributes.AttackUp();
            if (playerAttributes.damageLevel >= 5)
            {
                attackUpButton.GetComponent<Button>().interactable = false;
                attackLevelText.text = "Maxed Out";
            }
            else
            {
                attackLevelText.text = "Level:" + playerAttributes.damageLevel.ToString();
            }
            if (playerAttributes.leftSkillPoint <= 0)
            {
               CloseSkillChoosePanel();
            }
        }
    }
    public void IncreaseHealth()
    {
        if (playerAttributes.leftSkillPoint > 0)
        {
            playerAttributes.leftSkillPoint--;
            playerAttributes.vitality++;
            playerAttributes.HpUp();
            if (playerAttributes.vitality >= 5)
            {
                hpUpButton.GetComponent<Button>().interactable = false;
                hpLevelText.text = "Maxed Out";
            }
            else
            {
                hpLevelText.text = "Level:" + playerAttributes.vitality.ToString();
            }
            if (playerAttributes.leftSkillPoint <= 0)
            {
                CloseSkillChoosePanel();
            }
        }
    }
    public void IncreaseSpeed()
    {
        if (playerAttributes.leftSkillPoint > 0)
        {
            playerAttributes.leftSkillPoint--;
            playerAttributes.speedLevel++;
            playerAttributes.SpeedUp();
            if (playerAttributes.speedLevel >= 5)
            {
                speedUpButton.GetComponent<Button>().interactable = false;
                speedLevelText.text = "Maxed Out";
            }
            else
            {
                speedLevelText.text = "Level:" + playerAttributes.speedLevel.ToString();
            }
            if (playerAttributes.leftSkillPoint <= 0)
            {
                CloseSkillChoosePanel();
            }
        }
    }
    public void IncreaseSpecial()
    {
        if (playerAttributes.leftSkillPoint > 0)
        {
            playerAttributes.leftSkillPoint--;
            playerAttributes.specialLevel++;
            if (playerAttributes.specialLevel >= 5)
            {
                specialUpButton.GetComponent<Button>().interactable = false;
                specialLevelText.text = "Maxed Out";
            }
            else
            {
                specialLevelText.text = "Level:" + playerAttributes.specialLevel.ToString();
            }
            if (playerAttributes.leftSkillPoint <= 0)
            {
                CloseSkillChoosePanel();
            }
        }
    }
    //stat decreasing section
    public void DecreaseInt()
    {
        if (playerAttributes.statPoints <= playerAttributes.playerLevel)
        {
            
            if (playerAttributes.intelligence > 1)
            {
                playerAttributes.intelligence--;
                playerAttributes.statPoints++;
            }
            
            UpdateText();
        }
    }
    public void DecreaseVit()
    {
        if (playerAttributes.statPoints <= playerAttributes.playerLevel)
        {
            
            if (playerAttributes.vitality > 1)
            {
                playerAttributes.vitality--;
                playerAttributes.statPoints++;
            }
            
            UpdateText();
        }
    }
}
