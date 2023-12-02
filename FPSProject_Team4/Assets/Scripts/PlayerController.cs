using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] CharacterController controller;

    public int HP; //configurable amt of HP
    [SerializeField] float PlayerSpeed; //configurable speed
    [SerializeField] float JumpHeight; //configurable jump height
    [SerializeField] float GravityValue;
    [SerializeField] int JumpMax; //configurable max amt of jumps 
    [SerializeField] float SprintMod; //configurable amt for speed multiplier

    [SerializeField] int ShootDamage; //configurable dmg amt
    [SerializeField] float ShootRate; //configurable rate of fire (per sec)
    [SerializeField] int ShootDist; //configurable distance of shots
    //[SerializeField] GameObject Cube; //for the projectile shot

    private Vector3 PlayerVelocity;
    private bool GroundedPlayer; //is player grounded or not
    private Vector3 Move;
    private int JumpCount; //amt of jumps player has currently remaining
    private bool IsShooting;
    public int HPOriginal; //default starting HP

    private void Start()
    {
        HPOriginal = HP; //sets default hp to player's current HP
        UIManager.instance.UpdatePlayerHP();
    }

    void Update()
    {
        movement();
        ShootingTimer();
    }

    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * ShootDist, Color.red); //gives red line for gun shooting distance in the scene NOT in game since its debug

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



        //lets the player jump if they have any left
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

    void ShootingTimer()
    {
        if (!GameManager.instance.isPaused && Input.GetButton("Shoot") && !IsShooting) //may shoot when unpaused
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        IsShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, ShootDist)) //.5 .5 is middle of screen
        {
            IDamageable dmg = hit.collider.GetComponent<IDamageable>(); //returns smth if it hits smth with IDamage

            if (dmg != null)
            {
                dmg.takeDamage(ShootDamage);
            }
        }

        yield return new WaitForSeconds(ShootRate);
        IsShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount; //player takes dmg
        UIManager.instance.UpdatePlayerHP();

        if (HP <= 0)
        {
            //player dies
            GameManager.instance.YouLose();
        }
    }
}
