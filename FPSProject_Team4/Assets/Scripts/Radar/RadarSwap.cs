using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSwap : MonoBehaviour
{
    [Header("----- Radar Swap -----")]
    private GameObject radarWide;
    private GameObject radarNarrow;
    private RadarPulse pulse;
    private Camera radarCam;
    private PlayerController player;
    
    // Start is called before the first frame update
    void Start()
    {
        radarWide = GameObject.Find("Radar/RadarWindow/RadarWide");
        radarNarrow = GameObject.Find("Radar/RadarWindow/RadarNarrow");
        pulse = GameObject.Find("PlayerRadar").GetComponent<RadarPulse>();
        radarCam = GameObject.Find("PlayerRadar/RadarCamera").GetComponent<Camera>();
        player = GameManager.instance.playerScript;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isADS)
        {
            RadarN();
        }
        else
        {
            RadarW();
        }
        
        if (player.isSprinting)
        {
            sprintView();
        }
        else
        {
            normView();
        }
    }

    void RadarW()
    {
        radarNarrow.SetActive(false);
        radarWide.SetActive(true);
    }

    void RadarN()
    {
        radarWide.SetActive(false);
        radarNarrow.SetActive(true);
    }

    void normView()
    {
        radarCam.orthographicSize = 45;
        pulse.maxRange = 40;
    }

    void sprintView()
    {
        radarCam.orthographicSize = 60;
        pulse.maxRange = 55;
    }
}
