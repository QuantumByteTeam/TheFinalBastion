using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    /*[SerializeField] string volumeParameter;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider volumeSlider;
    [SerializeField] float value;

    private void Awake()
    {
        volumeSlider.onValueChanged.AddListener(sliderChanges);
    }

    private void sliderChanges(float val)
    {
        audioMixer.SetFloat("Master", val);
        //audioMixer.SetFloat("Master", Mathf.Log10(val) * value);
    }

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(volumeParameter, volumeSlider.value);
    }*/

    public AudioMixer audioMixer;
    public AudioMixer GetMixer() { return audioMixer; }

    public void SetSlidertoVolume()
    {
        //make it so that I set the volume slider to the volume to prevent value disconnect
    }

    public void SetVolume(float volume)
    {
        Debug.Log(volume);
        audioMixer.SetFloat("volume", volume);
    }
}
