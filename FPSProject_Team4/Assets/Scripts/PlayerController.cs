using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioMixerGroup audioMixerGroup;
    [SerializeField] Animator anim;

    [Header("----- Stats -----")]
    public float HP; //configurable amt of HP
    public float PlayerSpeed; //configurable speed
    [SerializeField] float JumpHeight; //configurable jump height
    [SerializeField] float GravityValue;
    [SerializeField] int JumpMax; //configurable max amt of jumps 
    [SerializeField] float SprintMod; //configurable amt for speed multiplier
    [SerializeField] float animSpeedTransition; //anim speed
    [SerializeField] private Vector3 crouchingScale = new Vector3(1, 0.5f, 1);
    [SerializeField] private Vector3 standingScale = new Vector3(1, 1, 1);

    CharacterController colliderHeight;


    [Header("----- Weapon -----")]
    public List<gunStats> gunList = new List<gunStats>();
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
    public bool IsShooting;
    public float HPOriginal; //default starting HP (changed to float)
    [SerializeField] Transform ADSPosition;
    [SerializeField] Transform gunPosition;

    int SelectedGun; //current gun the player is holding
    bool isPlayingSteps;
    bool isWalking;
    public bool isSprinting;
    bool isJumping;
    public bool isADS;
    bool isPlayingEmpty;
    bool isPlayingReload;
    bool isPlayingShoot;
    bool isCrouching;
    bool isHealing;

    Coroutine lastShoot = null;
    Coroutine lastEmpty = null;
    Coroutine lastReload = null;
    Coroutine lastSound = null;

    bool shootSwap;

    [SerializeField] Transform uzi;
    [SerializeField] Transform m4;
    [SerializeField] Transform sg;
    [SerializeField] ParticleSystem flash;

    public float damageModifier;

    //added by John
    public int ammoCount;
    public int ammoMag;
    public int ammoReserve;
    private bool reloading;
    public bool armor; //was priv

    public int SelectedItem;
    public playerInventory inventory = new playerInventory();
    private int invSize;
    public bool swap;
    [SerializeField] float FOV;
    
    private void Start()
    {
        aud.outputAudioMixerGroup = audioMixerGroup;
        HPOriginal = HP; //sets default hp to player's current HP
        respawnPlayer();
        controller.enabled = true;


    }

    void Update()
    {
        invSize = inventory.hotbarInventory.Count();
        if (invSize > 0)
        {
            //holdingGun = inventory.hotbarInventory.ElementAt(SelectedItem).Key.isGun;
            if (SelectedItem >= invSize)
            {
                SelectedItem = invSize - 1;
            }
        }



        if (!GameManager.instance.isActivePaused) //checks if game is Actve paused (interaction menus), if paused it doesnt call anything below
        {
            if (!GameManager.instance.isPaused) //checks if game is paused, if paused it doesnt call anything below
            {
                if (anim.isActiveAndEnabled)
                {


                    float animSpeed = anim.velocity.normalized.magnitude;

                    anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTransition));



                    if (Input.GetButton("ADS") && inventory.hotbarInventory.ElementAt(SelectedItem).Key.isGun)
                    {
                        isADS = true;
                        Camera.main.fieldOfView = FOV * 0.75f;
                        GunModel.transform.position = ADSPosition.transform.position;
                        GunModel.transform.rotation = ADSPosition.transform.rotation;
                    }
                    else
                    {
                        isADS = false;
                        Camera.main.fieldOfView = FOV;
                        GunModel.transform.position = gunPosition.transform.position;
                        GunModel.transform.rotation = gunPosition.transform.rotation;
                    }


                    if (Input.GetButtonDown("Drop") && invSize > 0)
                    {
                        inventory.drop(SelectedItem);
                    }

                    if (invSize >= 0)
                    {
                        SelectItem();
                    }

                    if (inventory.hotbarInventory.Count > 0 && inventory.hotbarInventory.ElementAt(SelectedItem).Key.isGun)
                    {
                        if (Input.GetButton("Shoot") && !IsShooting && !reloading/* && !swap*/)
                        {
                            IsShooting = true;
                            //shootSwap = true;
                            lastShoot = StartCoroutine(Shoot());
                        }

                        if (Input.GetButton("Reload") && !IsShooting)
                        {
                            lastReload = StartCoroutine(reload());
                        }
                        //SelectGun();
                    }
                    else if (inventory.hotbarInventory.Count > 0 && inventory.hotbarInventory.ElementAt(SelectedItem).Key.isDeployable)
                    {
                        if (Input.GetButtonDown("Shoot"))
                        {
                            //IsShooting = true;
                            //shootSwap = true;
                            Instantiate(inventory.hotbarInventory.ElementAt(SelectedItem).Key.deployable, Camera.main.transform.position + (Camera.main.transform.forward * inventory.hotbarInventory.ElementAt(SelectedItem).Key.deployDistance), Camera.main.transform.rotation);
                            inventory.Remove(SelectedItem);
                            UIManager.instance.updateHotbar();
                            UIManager.instance.UpdateAmmo();
                        }

                    }

                    if (Input.GetButtonUp("Shoot"))
                    {
                        //IsShooting = false;
                        //shootSwap = false;
                        //swap = false;
                    }
                    controller.enabled = true; //Prevents bug where controller gets disabled for some reason
                    movement();
                }
            }
        }
    }

    IEnumerator playSteps()
    {
        //aud.outputAudioMixerGroup = audioMixer.outputAudioMixerGroup;
        isPlayingSteps = true;
        aud.PlayOneShot(SoundSteps[UnityEngine.Random.Range(0, SoundSteps.Length - 1)], SoundStepsVol);


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
        crouch();

        GroundedPlayer = controller.isGrounded;

        if (GroundedPlayer && Move.normalized.magnitude > 0.3f && !isPlayingSteps) //can also use a Vector3.0 in place ofthe normalized and mag but it will start playing as soon as u move
        {
            StartCoroutine(playSteps());
        }

        if (GroundedPlayer)
        {
            isJumping = false;
        }



        if (Move.normalized.magnitude > 0.3f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
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
            isJumping = true;
            PlayerVelocity.y = JumpHeight;
            aud.PlayOneShot(SoundJumps[UnityEngine.Random.Range(0, SoundJumps.Length - 1)], SoundJumpsVol); //plays jump sfx randomly
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
    void crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            colliderHeight = GetComponent<CharacterController>();
            colliderHeight.height = 1.0f;
            isCrouching = true;
        }

        else if (Input.GetButtonUp("Crouch"))
        {
            colliderHeight = GetComponent<CharacterController>();
            colliderHeight.height = 2.0f;
            isCrouching = false;
        }
    }
    void ShootingTimer()
    {
        if (!GameManager.instance.isPaused && Input.GetButton("Shoot") && !IsShooting)
        {
            StartCoroutine(Shoot());
        }
    }



    IEnumerator Shoot()
    {
        inventoryItem currentItem = inventory.hotbarInventory.ElementAt(SelectedItem).Key;



        if (currentItem.ammoCount > 0)
        {
            if (!isPlayingShoot && currentItem.isGun/* && !swap*/)
            {
                lastSound = StartCoroutine(ShootSound());
            }
            if (currentItem.ID == 0)
            {
                Instantiate(flash, sg.position, Quaternion.LookRotation(GunModel.transform.forward * -1));
            }
            else if (currentItem.ID == 1)
            {
                Instantiate(flash, m4.position, Quaternion.LookRotation(GunModel.transform.forward * -1));
            }
            else if (currentItem.ID == 2)
            {
                Instantiate(flash, uzi.position, Quaternion.LookRotation(GunModel.transform.forward * -1));
            }
            

            //Camera.main.transform.Rotate(Vector3.right, currentItem.recoil);

            ammoCount--;
            currentItem.ammoCount--;
            RaycastHit hit;
            
            float bA = currentItem.baseAccuracy;

            if (isSprinting)
            {
                bA = bA * currentItem.runningMultiplier;
            }
            if (isWalking)
            {
                bA = bA * currentItem.walkingMultiplier;
            }
            if (isCrouching)
            {
                bA = bA * currentItem.crouchingMultiplier;
            }
            if (isJumping)
            {
                bA = bA * currentItem.jumpingMultiplier;
            }
            if (isADS)
            {
                bA = bA * currentItem.adsMultiplier;
            }

            int temp = 1;

            if (inventory.hotbarInventory.ElementAt(SelectedItem).Key.isShotgun)
            {
                temp = inventory.hotbarInventory.ElementAt(SelectedItem).Key.numberOfShot;
            }
            for (int i = 0; i < temp; i++)
            {
                Vector3 fwd = Camera.main.transform.forward;
                fwd = fwd + Camera.main.transform.TransformDirection(new Vector3(UnityEngine.Random.Range(-bA, bA), UnityEngine.Random.Range(-bA, bA)));
                Debug.DrawRay(Camera.main.transform.position, fwd, Color.cyan, 10);
                if (Physics.Raycast(Camera.main.transform.position, fwd, out hit, currentItem.ShootDist))
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        Instantiate(inventory.hotbarInventory.ElementAt(SelectedItem).Key.BloodEffect, hit.point, transform.rotation); //gun spark particle
                    }
                    else if (hit.collider.CompareTag("Pipe") && hit.collider.GetComponent<PipeSystem>().isWater)
                    {
                        StartCoroutine(WaterLeakEffect(hit.collider.transform, hit.point, transform.rotation));
                    }
                    else if (hit.collider.CompareTag("Pipe") && (hit.collider.GetComponent<PipeSystem>().isSmoke || hit.collider.GetComponent<PipeSystem>().isGas))
                    {
                        StartCoroutine(GasLeakEffect(hit.collider.transform, hit.point, transform.rotation));
                    }
                    else
                    {
                        Instantiate(inventory.hotbarInventory.ElementAt(SelectedItem).Key.HitEffect, hit.point, transform.rotation); //gun spark particle
                    }

                    IDamageable dmg = hit.collider.GetComponent<IDamageable>();

                    if (hit.transform != transform && dmg != null && hit.collider.tag != "Point")
                    {
                        float falloff = inventory.hotbarInventory.ElementAt(SelectedItem).Key.damageFalloffPerMeter * Vector3.Distance(Camera.main.transform.position, hit.point);
                        dmg.takeDamage(inventory.hotbarInventory.ElementAt(SelectedItem).Key.ShootDamage * damageModifier * (1 - falloff), inventory.hotbarInventory.ElementAt(SelectedItem).Key.armorPen);
                    }
                }
            }
            
            UIManager.instance.UpdateAmmo();
            IsShooting = true;
            yield return new WaitForSeconds(inventory.hotbarInventory.ElementAt(SelectedItem).Key.ShootRate);
            
            IsShooting = false;

            //if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, inventory.hotbarInventory.ElementAt(SelectedItem).Key.ShootDist))
            //{
            //    if (hit.collider.gameObject.GetComponent<ImpactAudio>())
            //    {
            //        hit.collider.gameObject.GetComponent<ImpactAudio>().playImpact();
            //    }
            //}

            //Causing soundHurt to play whenever an enemy is shot
        }
        else
        {
            if (!isPlayingEmpty)
            {
                lastEmpty = StartCoroutine(playEmptySound());
            }
            IsShooting = false;
        }


    }

    IEnumerator ShootSound()
    {
        if(inventory.hotbarInventory.ElementAt(SelectedItem).Key.isGun/* && !swap*/)
        {
            isPlayingShoot = true;
            aud.PlayOneShot(inventory.hotbarInventory.ElementAt(SelectedItem).Key.ShootSound[UnityEngine.Random.Range(0, inventory.hotbarInventory.ElementAt(SelectedItem).Key.ShootSound.Length - 1)], inventory.hotbarInventory.ElementAt(SelectedItem).Key.ShootSoundVol); //plays the associated gun noise each time a bullet is shot
            yield return new WaitForSeconds(inventory.hotbarInventory.ElementAt(SelectedItem).Key.ShootRate);
            aud.PlayOneShot(inventory.hotbarInventory.ElementAt(SelectedItem).Key.CasingSound[UnityEngine.Random.Range(0, inventory.hotbarInventory.ElementAt(SelectedItem).Key.CasingSound.Length - 1)], inventory.hotbarInventory.ElementAt(SelectedItem).Key.CasingSoundVol); //plays the associated bullet casing drop noise each time a bullet is shot
            isPlayingShoot = false;
        }
    }

    IEnumerator playEmptySound()
    {
        isPlayingEmpty = true;
        aud.PlayOneShot(inventory.hotbarInventory.ElementAt(SelectedItem).Key.EmptySound[UnityEngine.Random.Range(0, inventory.hotbarInventory.ElementAt(SelectedItem).Key.EmptySound.Length - 1)], inventory.hotbarInventory.ElementAt(SelectedItem).Key.EmptySoundVol); //plays the no ammo empty gun click sound
        yield return new WaitForSeconds(inventory.hotbarInventory.ElementAt(SelectedItem).Key.ShootRate);
        isPlayingEmpty = false;

    }
    
    IEnumerator WaterLeakEffect(Transform parent, Vector3 pos, Quaternion rotation)
    {
        rotation *= Quaternion.Euler(0,180f,0);
        
        Instantiate(inventory.hotbarInventory.ElementAt(SelectedItem).Key.HitEffect, pos, rotation);

        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(0.5f);
        
        Instantiate(parent.gameObject.GetComponent<PipeSystem>().waterHitEffect, pos, rotation, parent);
    }

    IEnumerator GasLeakEffect(Transform parent, Vector3 pos, Quaternion rotation)
    {
        rotation *= Quaternion.Euler(0,180f,0);
        
        Instantiate(inventory.hotbarInventory.ElementAt(SelectedItem).Key.HitEffect, pos, rotation);

        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(0.5f);
        
        Instantiate(parent.gameObject.GetComponent<PipeSystem>().vapourHitEffect, pos, rotation, parent);
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
        Debug.Log("Playing Hurt");
        aud.PlayOneShot(SoundHurt[UnityEngine.Random.Range(0, SoundHurt.Length - 1)], SoundHurtVol); //plays audio randomly from the whole range of tracks when player hurt
        UIManager.instance.UpdatePlayerHP();

        if (HP <= 0)
        {
            //player dies
            GameManager.instance.YouLose();
        }
        else
        {
            if (!isHealing)
            {
                StartCoroutine(Cooldown());
            }
        }
    }

    IEnumerator playerFlashDamage() //flashes the red panel screen when dmg is taken
    {
        UIManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(.1f);
        UIManager.instance.playerDamageScreen.SetActive(false);
    }

    void PlayerRegen()
    {
        if (HP > (HPOriginal * 0.75f))
        {
            HP = HPOriginal;
        }
        else if (HP > (HPOriginal * 0.5f))
        {
            HP = HPOriginal * 0.75f;
        }
        else if (HP > (HPOriginal * 0.25f))
        {
            HP = HPOriginal * 0.5f;
        }
        else
        {
            HP = HPOriginal * 0.25f;
        }

        UIManager.instance.UpdatePlayerHP();
    }

    IEnumerator Cooldown()
    {
        isHealing = true;
        yield return new WaitForSeconds(10);
        PlayerRegen();
        isHealing = false;
    }

    public void GetGunStats(inventoryItem item) //gives the current picked up/equipped gun the proper stats
    {
        if (inventory.hotbarInventory.ContainsKey(item))
        {
            inventory.hotbarInventory[item] += 1;
        }
        else
        {
            inventory.hotbarInventory.Add(item, 1);
        }


        //StopAllCoroutines(); // <<<<<<<<<<<<<<<<<<<<<<<<<<< may be a cause of error in the future
        
        //StartCoroutine(playSteps());

        if (inventory.hotbarInventory.Count == 1)
        {
            ChangeItem();
        }
            


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

    void SelectItem()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && SelectedItem > 0 && !IsShooting)//scrolling up, -1 so that ur one less than out of bounds
        {
            SelectedItem--;
            UIManager.instance.updateSelection(SelectedItem);
            if (inventory.hotbarInventory.ElementAt(SelectedItem).Key.isGun)
            {
                SelectedGun--;
            }
            ChangeItem();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && SelectedItem < inventory.hotbarInventory.Count - 1 && !IsShooting) //scrolling down, makes sure we never get past 0
        {
            SelectedItem++;
            UIManager.instance.updateSelection(SelectedItem);
            if (inventory.hotbarInventory.ElementAt(SelectedItem).Key.isGun)
            {
                SelectedGun++;
            }
            ChangeItem();
        }
    }


    public void ChangeItem()
    {
        //StopAllCoroutines();
        if (lastReload != null) //doesn't really work
        {
            StopCoroutine(lastReload);
        }
        
        if (lastShoot != null) //this one does
        {
            StopCoroutine(lastShoot);
        }

        UIManager.instance.reloadingText.SetActive(false);
        inventoryItem currentGun;
        if (inventory.hotbarInventory.Count > 0)
        {
            currentGun = inventory.hotbarInventory.ElementAt(SelectedItem).Key;
        }
        else
        {
            currentGun = null;
        }
        

        


        IsShooting = false;
        isPlayingShoot = false;
        reloading = false;

        GunModel.GetComponent<MeshFilter>().sharedMesh = null;
        GunModel.GetComponent<MeshRenderer>().sharedMaterial = null;
        GunMag.GetComponent<MeshFilter>().sharedMesh = null;
        GunMag.GetComponent<MeshRenderer>().sharedMaterial = null;
        GunTrig.GetComponent<MeshFilter>().sharedMesh = null;
        GunTrig.GetComponent<MeshRenderer>().sharedMaterial = null;

        if (inventory.hotbarInventory.Count > 0 && currentGun.Model != null)
        {
            GunModel.GetComponent<MeshFilter>().sharedMesh = currentGun.Model.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun model
            GunModel.GetComponent<MeshRenderer>().sharedMaterial = currentGun.Model.GetComponent<MeshRenderer>().sharedMaterial; //sets the texture/shar to the correct gun
            float temp = currentGun.modelScale;
            GunModel.transform.localScale = new Vector3(temp, temp, temp);
        }



        if (inventory.hotbarInventory.Count > 0 && currentGun.isGun)
        {
            ChangeGun();
        }

        UIManager.instance.updateSelection(SelectedItem);
    }

    void ChangeGun() //has double pump exploit, BUG WHEN A GUN IS PICKEDUP FIRERATE DOUBLES, temp fix by moving the vars into get stats
    {
        inventoryItem currentGun = inventory.hotbarInventory.ElementAt(SelectedItem).Key;
        //if (shootSwap)
        //{
        //    swap = true;
        //}

        IsShooting = false;
        isPlayingShoot = false;
        reloading = false;
        //isPlayingSteps = false; //julius commented this out since it caused the player to hear double audio when picking up a gun
        armorPen = currentGun.armorPen;
        ShootDamage = currentGun.ShootDamage;
        ShootRate = currentGun.ShootRate;
        ShootDist = currentGun.ShootDist;

        if (currentGun.firstInstance)
        {
            ammoCount = currentGun.ammoMag;
            currentGun.ammoCount = currentGun.ammoMag;
            ammoReserve = currentGun.ammoReserveDefault;
            currentGun.ammoReserve = currentGun.ammoReserveDefault;
            currentGun.firstInstance = false;
        }
        else
        {
            ammoCount = currentGun.ammoCount;
            ammoReserve = currentGun.ammoReserve;
        }

        ammoMag = currentGun.ammoMag;

        //gun models

        //gun mags
        GunMag.GetComponent<MeshFilter>().sharedMesh = currentGun.MagModel.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun mag
        GunMag.GetComponent<MeshRenderer>().sharedMaterial = currentGun.MagModel.GetComponent<MeshRenderer>().sharedMaterial; //sets the mag texture/renderer
        //gun triggers
        GunTrig.GetComponent<MeshFilter>().sharedMesh = currentGun.TrigModel.GetComponent<MeshFilter>().sharedMesh; //sets the model to the correct gun mag
        GunTrig.GetComponent<MeshRenderer>().sharedMaterial = currentGun.TrigModel.GetComponent<MeshRenderer>().sharedMaterial; //sets the mag texture/renderer

        UIManager.instance.UpdateAmmo();
        UIManager.instance.updateHotbar();

        IsShooting = false;

    }


    IEnumerator reload()
    {
        reloading = true;
        // aud.PlayOneShot(gunList[SelectedGun].ReloadSound[Random.Range(0,gunList[SelectedGun].ReloadSound.Length - 1)], gunList[SelectedGun].ReloadSoundVol); //plays the associated gun reload sound

        inventoryItem currentGun = inventory.hotbarInventory.ElementAt(SelectedItem).Key;

        if (currentGun.ammoReserve > 0 && currentGun.ammoCount < currentGun.ammoMag)
        {
            if (!isPlayingReload)
            {
                StartCoroutine(playReloadSound());
            }
            StartCoroutine(UIManager.instance.reloading(currentGun.reloadTime));

            //reloading = true;
            yield return new WaitForSeconds(currentGun.reloadTime);
            if (reloading)
            {
                if (currentGun.ammoReserve >= currentGun.ammoMag - currentGun.ammoCount)
                {
                    currentGun.ammoReserve -= currentGun.ammoMag - currentGun.ammoCount;
                    currentGun.ammoCount = currentGun.ammoMag;
                }
                else
                {
                    currentGun.ammoCount += currentGun.ammoReserve;
                    currentGun.ammoReserve = 0;
                }
                ammoCount = currentGun.ammoCount;
                ammoMag = currentGun.ammoMag;
                ammoReserve = currentGun.ammoReserve;
                UIManager.instance.UpdateAmmo();
            }
        }
        else
        {
            yield return new WaitForSeconds(0);
            
        }
        
        
        
        reloading = false;
    }

    IEnumerator playReloadSound()
    {
        isPlayingReload = true;
        aud.PlayOneShot(inventory.hotbarInventory.ElementAt(SelectedItem).Key.ReloadSound[UnityEngine.Random.Range(0, inventory.hotbarInventory.ElementAt(SelectedItem).Key.ReloadSound.Length - 1)], inventory.hotbarInventory.ElementAt(SelectedItem).Key.ReloadSoundVol); //plays the associated gun reload sound
        yield return new WaitForSeconds(.25f); //sprint pace
        isPlayingReload = false;

    }

}
