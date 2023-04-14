using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class PlayerAttributes : MonoBehaviourPunCallbacks
{
    public static PlayerAttributes instance;

    public float moveSpeed;
    private Vector2 moveInput;

    public Rigidbody2D playerRB;

    public Transform playerWeaponHand;

    private Camera playerCamera;

    public Animator playerAnimator;



    //Stats
    public int strength = 1;
    public int dexterity = 1;
    public int intelligence = 1;
    public int vitality= 1;
    public int statPoints = 0;
    public PlayerData.Classes playerClass;
    public Dictionary<PlayerData.Classes,int[]> classAndStats;
    public ClassScriptable playerclassscriptable;

    //Exp
    public int playerMaxExperience = 100;
    public int playerCurrentExperience = 1;
    public int playerLevel = 1; 
    
    //Gold
    public int playerGold = 0;
    public delegate void OnGetExp();
    public static OnGetExp onGetExp;

    public int playerMaxHealth = 1000;
    public int playerCurrentHealth;

    public GameObject hitEffect;

    private void Awake()
    {
        instance = this;
        classAndStats = new Dictionary<PlayerData.Classes, int[]>();
    }

    void Start()
    {
        
        if (photonView.IsMine)
        {
            playerCamera = Camera.main;
            if (classAndStats == new Dictionary<PlayerData.Classes, int[]>())
            {
                CreateClassesDictionary();
            }
        }

        playerCurrentHealth = playerMaxHealth;

        UIController.instance.healthSlider.maxValue = playerMaxHealth;
        UIController.instance.healthSlider.value = playerMaxHealth;
    }

    void Update()
    {
        if(photonView.IsMine)
        { 
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            playerRB.velocity = moveInput * moveSpeed;

            Vector3 mousePosition = Input.mousePosition;
            Vector3 screenPoint = playerCamera.WorldToScreenPoint(transform.localPosition);

            if(mousePosition.x < screenPoint.x)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                playerWeaponHand.localScale = new Vector3(-1f, -1f, 1f);
            }
            else
            {
                transform.localScale = Vector3.one;
                playerWeaponHand.localScale = Vector3.one;
            }

            Vector2 weaponOffset = new Vector2(mousePosition.x - screenPoint.x, mousePosition.y - screenPoint.y);
            float weaponAngle = Mathf.Atan2(weaponOffset.y, weaponOffset.x) * Mathf.Rad2Deg;
            playerWeaponHand.rotation = Quaternion.Euler(0, 0, weaponAngle);

            if(moveInput != Vector2.zero)
            {
                playerAnimator.SetBool("isPlayerMoving", true);
            }
            else
            {
                playerAnimator.SetBool("isPlayerMoving", false);
            }
        }
    }

    public void SetStats()
    {
        int[] stats = new int[4];
        stats = classAndStats[playerclassscriptable.currentClass];
        strength = stats[0];
        dexterity = stats[1];
        intelligence = stats[2];
        vitality = stats[3];
    }
    public void SetLoadedData(PlayerData data, PlayerAttributes playerAttributes)
    {
        playerAttributes.playerLevel = data.level;
        playerAttributes.playerGold = data.gold;
        playerAttributes.statPoints = data.remainingStats;
        playerAttributes.classAndStats = data.classAndStats;
        playerAttributes.playerCurrentExperience = data.currentExp;
        playerAttributes.playerMaxExperience = data.maxExp;
        ChangeClass(playerclassscriptable);
    }

    public void CreateClassesDictionary()
    {
        int[] stats = new[] { 1, 1, 1, 1 };
        classAndStats[PlayerData.Classes.archer] = stats;
        classAndStats[PlayerData.Classes.mage] = stats;
        classAndStats[PlayerData.Classes.warrior] = stats;
    }
    public void AddStatToClass(PlayerData.Classes pClass)
    {
        
        int[] stats = new int[4];
        stats[0] = strength;
        stats[1] = dexterity;
        stats[2] = intelligence;
        stats[3] = vitality;

        
        classAndStats[pClass] = stats;
        
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if(photonView.IsMine)
        {
            playerCurrentHealth -= damage;
            PhotonNetwork.Instantiate(hitEffect.name, transform.position, transform.rotation);

            if (playerCurrentHealth <= 0)
            {
                playerCurrentHealth = 0;
                PlayerSpawner.instance.Die();
            }

            UIController.instance.healthSlider.value = playerCurrentHealth;
        }
    }
    //Level Up
    public void LevelUp()
    {
        playerLevel++;
        statPoints++;
        playerCurrentExperience -= playerMaxExperience; 
        playerMaxExperience += (int)(playerMaxExperience*20 / 100);
        if (playerCurrentExperience >= playerMaxExperience)
        {
            LevelUp();
        }
        onGetExp += SkillTreeController.instance.UpdateExpValuesOnText;
        onGetExp?.Invoke();
        onGetExp -= SkillTreeController.instance.UpdateExpValuesOnText;
    }
    //Get exp
    public void GetExp(int amount)
    {
        playerCurrentExperience += amount;
        onGetExp += SkillTreeController.instance.UpdateExpValuesOnText;
        onGetExp?.Invoke();
        if (playerCurrentExperience >= playerMaxExperience)
        {
            LevelUp();
        }

        onGetExp -= SkillTreeController.instance.UpdateExpValuesOnText;
    }

    public void ChangeClass(ClassScriptable classScriptable)
    {
        playerClass = classScriptable.currentClass;
    }

    public void SavePlayer()
    {
        AddStatToClass(playerclassscriptable.currentClass);
        SaveSystem.SavePlayerData(this);
    }

    public PlayerData LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadData();
        if (!data.IsUnityNull())
        {
            SetLoadedData(data,this);
            SetStats();
            return data;
        }
        return null;
    }

   
}
