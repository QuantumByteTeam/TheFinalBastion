using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    public AudioSource menuAudio;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void HoverSound()
    {
        menuAudio.PlayOneShot(hoverSound);
    }
    public void ClickSound()
    {
        menuAudio.PlayOneShot(clickSound);
    }
}
