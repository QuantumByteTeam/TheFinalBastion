using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] string volumeParameter;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider volumeSlider;
    [SerializeField] float value = 30f;

    private void Awake()
    {
        volumeSlider.onValueChanged.AddListener(sliderChanges);
    }

    private void sliderChanges(float val)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(val) * value);
    }

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(volumeParameter, volumeSlider.value);
    }

}
