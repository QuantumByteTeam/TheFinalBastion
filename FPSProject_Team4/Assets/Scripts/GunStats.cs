using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu] //lets u right click on the prefab menu to create gun stats at the top


public class gunStats : ScriptableObject
{
    [Header("----- Stats -----")]
    public int ShootDamage;
    public float ShootRate;
    public int ShootDist;

    public int ammoCount;
    public int ammoMag;
    public int ammoReserve;

    public int ammoReserveDefault;

    public float armorPen;
    public float reloadTime;

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


    //add bools for ads/coach gun logic stuff 

}
