using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactAudio : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] AudioSource aud;
    
    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] ImpactSound;
    [Range(0, 1)][SerializeField] float ImpactSoundVol;

    public void playImpact()
    {
        aud.PlayOneShot(ImpactSound[Random.Range(0, ImpactSound.Length - 1)], ImpactSoundVol);
    }
}
