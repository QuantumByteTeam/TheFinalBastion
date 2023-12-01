using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] CharacterController controller;

    [SerializeField] int HP; //configurable amt of HP
    [SerializeField] float PlayerSpeed; //configurable speed
    [SerializeField] float JumpHeight; //configurable jump height
    [SerializeField] float GravityValue;
    [SerializeField] int JumpMax; //configurable max amt of jumps 
    [SerializeField] float SprintMod; //configurable amt for speed multiplier

    /*[SerializeField] int ShootDamage; //configurable dmg amt
    [SerializeField] float ShootRate; //configurable rate of fire (per sec)
    [SerializeField] int ShootDist; //configurable distance of shots
    [SerializeField] GameObject cube; //for the projectile shot*/

    private Vector3 PlayerVelocity;
    private bool GroundedPlayer; //is player grounded or not
    private Vector3 Move;
    private int JumpCount; //amt of jumps player has currently remaining
    //private bool IsShooting;
    int HPOriginal; //default starting HP

    private void Start()
    {
        HPOriginal = HP; //sets default hp to player's current HP
        //updatePlayerUI();
    }

    void Update()
    {
        movement();
    }

    void movement()
    {
        


        sprint();

        
        GroundedPlayer = controller.isGrounded;
        if (GroundedPlayer && PlayerVelocity.y < 0) //makes sure we dont fast fall (falls at normal speed)
        {
            PlayerVelocity.y = 0f;
            JumpCount = 0;
        }

        Move = Input.GetAxis("Horizontal") * transform.right +
               Input.GetAxis("Vertical") * transform.forward;

        controller.Move(Move * Time.deltaTime * PlayerSpeed);



        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && JumpCount < JumpMax)
        {
            PlayerVelocity.y = JumpHeight;
            JumpCount++;
        }

        PlayerVelocity.y += GravityValue * Time.deltaTime;
        controller.Move(PlayerVelocity * Time.deltaTime);

    }

    


    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            PlayerSpeed *= SprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            PlayerSpeed /= SprintMod;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
    }







}
