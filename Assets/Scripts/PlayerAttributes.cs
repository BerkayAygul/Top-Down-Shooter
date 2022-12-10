using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttributes : MonoBehaviourPunCallbacks
{
    public static PlayerAttributes instance;

    public float moveSpeed;
    private Vector2 moveInput;

    public Rigidbody2D playerRB;

    public Transform playerWeaponHand;

    private Camera playerCamera;

    public Animator playerAnimator;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if(photonView.IsMine)
        {
            playerCamera = Camera.main;
        }
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
}
