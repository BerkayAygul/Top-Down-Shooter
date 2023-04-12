using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeController : MonoBehaviour
{
    [SerializeField] private GameObject skillTreePanel;

    public void OpenSkillTree()
    {
        if (!skillTreePanel.activeInHierarchy)
        {
            skillTreePanel.SetActive(true);
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
}
