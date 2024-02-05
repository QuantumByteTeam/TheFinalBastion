using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    //[SerializeField] string volumeParameter;
    //[SerializeField] AudioMixer audioMixer;
    //[SerializeField] Slider volumeSlider;
    //[SerializeField] float value;
    [SerializeField] Toggle fullscreen;

    bool isWebGL = false;
    bool isLoading = false;

    private void Awake()
    {
        isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;
        LoadPrefs();
    }

    //private void sliderChanges(float val)
    //{
    //    audioMixer.SetFloat("Master", val);
    //    //audioMixer.SetFloat("Master", Mathf.Log10(val) * value);
    //}

    //private void Start()
    //{
    //    volumeSlider.value = PlayerPrefs.GetFloat(volumeParameter, volumeSlider.value);
    //}

    private void Start()
    {
        fullscreen.onValueChanged.AddListener(delegate
        {
            OnDisplayModeChanged(fullscreen);
        });
    }

    private void OnDisable()
    {
        SavePrefs();
    }

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

    public void SavePrefs()
    {
        float volume = -1;
        audioMixer.GetFloat("volume", out volume);
        PlayerPrefs.SetFloat("volume", volume);
        if (!isWebGL)
        {
            PlayerPrefs.SetString("displayMode", Screen.fullScreenMode.ToString());
            PlayerPrefs.SetInt("activeDisplay", GetCurrentDisplayNumber());
        }

        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        isLoading = true;
        audioMixer.SetFloat("volume", PlayerPrefs.GetFloat("volume", 1));
        if (!isWebGL)
        {
            Screen.fullScreenMode = (FullScreenMode)PlayerPrefs.GetInt("displayMode", (int)FullScreenMode.FullScreenWindow);

            fullscreen.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;

            int displayNumber = PlayerPrefs.GetInt("activeDisplay", GetCurrentDisplayNumber());
            if (Display.displays.Length >= displayNumber + 1 && Display.displays[displayNumber] != null)
            {
                Display.displays[displayNumber].Activate();
            }
        }
        isLoading = false;
    }

    public void OnDisplayModeChanged(Toggle change)
    {
        if (isLoading) return;

        Debug.Log("Changing");

        if (!isWebGL)
        {
            if (change.isOn)
            {
                int displayNumber = GetCurrentDisplayNumber();
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Display.displays[displayNumber].Activate();
                Debug.Log("Fullscreen");
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                Debug.Log("Maximized");
            }
        }

        Debug.Log("Mode Changed: " + change.isOn.ToString());
    }

    public int GetCurrentDisplayNumber()
    {
        List<DisplayInfo> displayLayout = new List<DisplayInfo>();
        Screen.GetDisplayLayout(displayLayout);
        return displayLayout.IndexOf(Screen.mainWindowDisplayInfo);
    }
}
