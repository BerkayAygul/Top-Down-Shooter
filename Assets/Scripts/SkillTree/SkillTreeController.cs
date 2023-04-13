using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class SkillTreeController : MonoBehaviour
{
    [SerializeField] private GameObject skillTreePanel;
    private GameObject player;
    private PlayerAttributes playerAttributes;
    public TextMeshProUGUI dexText, strText, vitText, intText;
    public TextMeshProUGUI remainStatText;

    private void Start()
    {
        GetPlayer();
        
        Debug.Log("Girdi"); 
        playerAttributes = player.GetComponent<PlayerAttributes>();
        
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

    //stat increase section
    public void IncreaseDex()
    {
        if (playerAttributes.statPoints > 0)
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
}
