using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;

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
    [SerializeField] GameObject GunModel; //for gun model
    [SerializeField] GameObject GunMag; //for gun mags
    [SerializeField] GameObject GunTrig; //for gun triggers

    //Copying code from my project - John
    float armorPen;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] SoundHurt;
    [Range(0, 1)][SerializeField] float SoundHurtVol;
    [SerializeField] AudioClip[] SoundSteps;
    [Range(0, 1)][SerializeField] float SoundStepsVol;
    [SerializeField] AudioClip[] SoundJumps;
    [Range(0, 1)][SerializeField] float SoundJumpsVol;

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
        if (!GameManager.instance.isPaused) //checks if game is paused
        {


            if (gunList.Count > 0)
            {
                if (Input.GetButton("Shoot") && !IsShooting && !reloading)
                {
                    StartCoroutine(Shoot());
                }

                if (Input.GetButton("Reload") && !IsShooting)
                {
                    StartCoroutine(reload());
                }
                SelectGun();
            }
            movement();
        }
    }

    IEnumerator playSteps()
    {
        isPlayingSteps= true;

        aud.PlayOneShot(SoundSteps[Random.Range(0, SoundSteps.Length - 1)], SoundStepsVol);

        if (!isSprinting) //not sprinting
        {
            yield return new WaitForSeconds(.45f); //normal pace

        }
        else
        {
            yield return new WaitForSeconds(.25f); //sprint pace
        }

        isPlayingSteps = false;

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

        if (GroundedPlayer && Move.normalized.magnitude > 0.3f && !isPlayingSteps) //can also use a Vector3.0 in place ofthe normalized and mag but it will start playing as soon as u move
        {
            StartCoroutine(playSteps());
        }


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
            aud.PlayOneShot(SoundJumps[Random.Range(0, SoundJumps.Length - 1)], SoundJumpsVol); //plays jump sfx randomly
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

            
            aud.PlayOneShot(gunList[SelectedGun].ShootSound, gunList[SelectedGun].ShootSoundVol); //plays the associated gun noise each time a bullet is shot

            ammoCount--;
            gunList[SelectedGun].ammoCount--;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, gunList[SelectedGun].ShootDist))
            {
                Instantiate(gunList[SelectedGun].HitEffect, hit.point, transform.rotation); //gun spark particle

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

        StartCoroutine(playerFlashDamage());
        aud.PlayOneShot(SoundHurt[Random.Range(0, SoundHurt.Length-1)], SoundHurtVol); //plays audio randomly from the whole range of tracks when player hurt
        UIManager.instance.UpdatePlayerHP();

        if (HP <= 0)
        {
            //player dies
            GameManager.instance.YouLose();
        }
    }

    IEnumerator playerFlashDamage() //flashes the red panel screen when dmg is taken
    {
        UIManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(.1f);
        UIManager.instance.playerDamageScreen.SetActive(false);
    }


    public void GetGunStats(gunStats gun) //gives the current picked up/equipped gun the proper stats
    {
        gunList.Add(gun); //adds each gun picked up to a list



        //sets the gun player just picked up to the gun's stats //moved this to ChangeGun()


        //ShootDamage = gun.ShootDamage;
        //ShootDist = gun.ShootDist;
        //ShootRate = gun.ShootRate;
        
        //gun models
        GunModel.GetComponent<MeshFilter>().sharedMesh = gun.Model.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun model
        GunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.Model.GetComponent<MeshRenderer>().sharedMaterial; //sets the texture/shar to the correct gun
        //gun mags
        GunMag.GetComponent<MeshFilter>().sharedMesh = gun.MagModel.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun mag
        GunMag.GetComponent<MeshRenderer>().sharedMaterial = gun.MagModel.GetComponent<MeshRenderer>().sharedMaterial; //sets the mag texture/renderer
        //gun triggers
        GunTrig.GetComponent<MeshFilter>().sharedMesh = gun.TrigModel.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun mag
        GunTrig.GetComponent<MeshRenderer>().sharedMaterial = gun.TrigModel.GetComponent<MeshRenderer>().sharedMaterial; //sets the mag texture/renderer

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
        //gun models
        GunModel.GetComponent<MeshFilter>().sharedMesh = gunList[SelectedGun].Model.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun model
        GunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[SelectedGun].Model.GetComponent<MeshRenderer>().sharedMaterial; //sets the texture/shar to the correct gun

        //gun mags
        GunMag.GetComponent<MeshFilter>().sharedMesh = gunList[SelectedGun].MagModel.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun mag
        GunMag.GetComponent<MeshRenderer>().sharedMaterial = gunList[SelectedGun].MagModel.GetComponent<MeshRenderer>().sharedMaterial; //sets the mag texture/renderer
        //gun triggers
        GunTrig.GetComponent<MeshFilter>().sharedMesh = gunList[SelectedGun].TrigModel.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun mag
        GunTrig.GetComponent<MeshRenderer>().sharedMaterial = gunList[SelectedGun].TrigModel.GetComponent<MeshRenderer>().sharedMaterial; //sets the mag texture/renderer



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
