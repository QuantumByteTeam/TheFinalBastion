using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "Inventory Item")]
public class inventoryItem : ScriptableObject
{
    public bool isGun;
    public bool isDeployable;
    public bool isConsumable;
    public bool isShotgun;
    public int numberOfShot;
    [SerializeField] Sprite itemSprite;
    public GameObject deployable;
    public GameObject droppedItem;

    public float deployDistance;

    public Sprite returnIcon()
    {
        return itemSprite;
    }

    [Header("----- Stats -----")]
    public int ShootDamage;
    public float ShootRate;
    public int ShootDist;
    public float damageFalloffPerMeter;

    public int ammoCount;
    public int ammoMag;
    public int ammoReserve;

    public int ammoReserveDefault;

    public float armorPen;
    public float reloadTime;

    
    public float crouchingMultiplier;
    public float adsMultiplier;
    public float walkingMultiplier;
    public float runningMultiplier;
    public float jumpingMultiplier;

    public float baseAccuracy; //in degrees from center


    public GameObject Model; //guns
    public GameObject MagModel; //magazines
    public GameObject TrigModel; //triggers
    public ParticleSystem HitEffect;
    public ParticleSystem BloodEffect;
    public AudioClip[] ShootSound; //was public 12/12
    [Range(0, 1)] public float ShootSoundVol;
    public AudioClip[] ReloadSound;
    [Range(0, 1)] public float ReloadSoundVol;
    public AudioClip[] CasingSound;
    [Range(0, 1)] public float CasingSoundVol;
    public AudioClip[] EmptySound;
    [Range(0, 1)] public float EmptySoundVol;

    [SerializeField] public float modelScale;
}