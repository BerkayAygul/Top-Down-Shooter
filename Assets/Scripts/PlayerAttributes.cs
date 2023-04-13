using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
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

    //Exp
    public int playerMaxExperience = 100;
    public int playerCurrentExperience = 1;
    public int playerLevel = 1; 
    public delegate void OnGetExp();
    public static OnGetExp onGetExp;

    public int playerMaxHealth = 1000;
    public int playerCurrentHealth;

    public GameObject hitEffect;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            playerCamera = Camera.main;
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

   
}
