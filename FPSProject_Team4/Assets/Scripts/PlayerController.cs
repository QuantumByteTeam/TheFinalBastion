using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Stats -----")]
    public float HP; //configurable amt of HP
    [SerializeField] float PlayerSpeed; //configurable speed
    [SerializeField] float JumpHeight; //configurable jump height
    [SerializeField] float GravityValue;
    [SerializeField] int JumpMax; //configurable max amt of jumps 
    [SerializeField] float SprintMod; //configurable amt for speed multiplier

    [Header("----- Weapon -----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] int ShootDamage; //configurable dmg amt
    [SerializeField] float ShootRate; //configurable rate of fire (per sec)
    [SerializeField] int ShootDist; //configurable distance of shots
    [SerializeField] GameObject GunModel;

    //Copying code from my project - John
    float armorPen;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] SoundHurt;
    [Range(0, 1)][SerializeField] float SoundHurtVol;
    [SerializeField] AudioClip[] Soundsteps;
    [Range(0, 1)][SerializeField] float SoundStepsVol;

    private Vector3 PlayerVelocity;
    private bool GroundedPlayer; //is player grounded or not
    private Vector3 Move;
    private int JumpCount; //amt of jumps player has currently remaining
    private bool IsShooting;
    public float HPOriginal; //default starting HP (changed to float)

    int SelectedGun; //current gun the player is holding
    bool isPlayingSteps;
    bool isSprinting;

    //added by John
    int ammoCount;
    int ammoMag;
    int ammoReserve;
    private bool reloading;
    private bool armor;


    private void Start()
    {
        HPOriginal = HP; //sets default hp to player's current HP
        respawnPlayer();
    }

    void Update()
    {
        //if (!GameManager.instance.isPaused) //checks if game is paused
        //{


            if (gunList.Count > 0)
            {
                if (Input.GetButton("Shoot") && !IsShooting)
                {
                    StartCoroutine(Shoot());
                }

                SelectGun();
            }
            movement();
        //}
    }


    public void respawnPlayer()
    {
        HP = HPOriginal;
        UIManager.instance.UpdatePlayerHP();

        controller.enabled = false; //this causes me (julius) to not be able to move in my own scene, but it works fine in the real scene
        transform.position = GameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
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
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            PlayerSpeed /= SprintMod;
            isSprinting = false;
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
        //IsShooting = true;

        //RaycastHit hit;
        //if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, ShootDist)) //.5 .5 is middle of screen
        //{
        //    IDamageable dmg = hit.collider.GetComponent<IDamageable>(); //returns smth if it hits smth with IDamage

        //    if (dmg != null)
        //    {
        //        dmg.takeDamage(ShootDamage, armorPen);
        //    }
        //}

        //yield return new WaitForSeconds(ShootRate);
        //IsShooting = false;

        if (gunList[SelectedGun].ammoCount > 0)
        {
            //audioSource.PlayOneShot(gunList[selectedGun].gunshot, gunList[selectedGun].gunshotVolume);
            ammoCount--;
            gunList[SelectedGun].ammoCount--;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, gunList[SelectedGun].ShootDist))
            {
                //Instantiate(gunList[SelectedGun].hitEffect, hit.point, transform.rotation);

                IDamageable dmg = hit.collider.GetComponent<IDamageable>();

                if (hit.transform != transform && dmg != null)
                {
                    dmg.takeDamage(gunList[SelectedGun].ShootDamage, gunList[SelectedGun].armorPen);
                }
            }

            IsShooting = true;
            yield return new WaitForSeconds(gunList[SelectedGun].ShootRate);
            IsShooting = false;
        }
    }

    public void takeDamage(float amount, float armorPen)
    {
        if (armor)
        {
            HP -= amount * armorPen;
        }
        else
        {
            HP -= amount;
        } //player takes dmg

        UIManager.instance.UpdatePlayerHP();

        if (HP <= 0)
        {
            //player dies
            GameManager.instance.YouLose();
        }
    }

    public void GetGunStats(gunStats gun) //gives the current picked up/equipped gun the proper stats
    {
        gunList.Add(gun); //adds each gun picked up to a list



        //sets the gun player just picked up to the gun's stats //moved this to ChangeGun()


        //ShootDamage = gun.ShootDamage;
        //ShootDist = gun.ShootDist;
        //ShootRate = gun.ShootRate;

        //GunModel.GetComponent<MeshFilter>().sharedMesh = gun.Model.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun model
        //GunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.Model.GetComponent<MeshRenderer>().sharedMaterial; //sets the texture/shar to the correct gun

        
        SelectedGun = gunList.Count - 1;
        ChangeGun();

    }

    void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && SelectedGun < gunList.Count - 1)//scrolling up, -1 so that ur one less than out of bounds
        {
            SelectedGun++;
            ChangeGun();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && SelectedGun > 0) //scrolling down, makes sure we never get past 0
        {
            SelectedGun--;
            ChangeGun();

        }

    } //picks which weapon to use via scroll wheel

    void ChangeGun() //has double pump exploit
    {
        ShootDamage = gunList[SelectedGun].ShootDamage;
        ShootDist = gunList[SelectedGun].ShootDist;
        ShootRate = gunList[SelectedGun].ShootRate;

        //John
        IsShooting = false;
        reloading = false;
        isPlayingSteps = false;
        armorPen = gunList[SelectedGun].armorPen;
        ShootDamage = gunList[SelectedGun].ShootDamage;
        ShootRate = gunList[SelectedGun].ShootRate;
        ShootDist = gunList[SelectedGun].ShootDist;
        ammoCount = gunList[SelectedGun].ammoCount;
        ammoMag = gunList[SelectedGun].ammoMag;
        ammoReserve = gunList[SelectedGun].ammoReserve;

        GunModel.GetComponent<MeshFilter>().sharedMesh = gunList[SelectedGun].Model.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun model
        GunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[SelectedGun].GetComponent<MeshRenderer>().sharedMaterial; //sets the texture/shar to the correct gun

        IsShooting = false;

    }


    IEnumerator reload()
    {
        reloading = true;
        if (gunList[SelectedGun].ammoReserve > 0 && gunList[SelectedGun].ammoCount < gunList[SelectedGun].ammoMag)
        {
            reloading = true;
            yield return new WaitForSeconds(gunList[SelectedGun].reloadTime);
            if (gunList[SelectedGun].ammoReserve >= gunList[SelectedGun].ammoMag - gunList[SelectedGun].ammoCount)
            {
                gunList[SelectedGun].ammoReserve -= gunList[SelectedGun].ammoMag - gunList[SelectedGun].ammoCount;
                gunList[SelectedGun].ammoCount = gunList[SelectedGun].ammoMag;
            }
            else
            {
                gunList[SelectedGun].ammoCount += gunList[SelectedGun].ammoReserve;
                gunList[SelectedGun].ammoReserve = 0;
            }
            reloading = false;
        }
        else
        {
            yield return new WaitForSeconds(0);
            reloading = false;
        }
        ammoCount = gunList[SelectedGun].ammoCount;
        ammoMag = gunList[SelectedGun].ammoMag;
        ammoReserve = gunList[SelectedGun].ammoReserve;
    }





}
