using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSwap : MonoBehaviour
{
    [Header("----- Radar Swap -----")]
    private GameObject radarWide;
    private GameObject radarNarrow;
    private PlayerController player;
    
    // Start is called before the first frame update
    void Start()
    {
        radarWide = GameObject.Find("Radar/RadarWindow/RadarWide");
        radarNarrow = GameObject.Find("Radar/RadarWindow/RadarNarrow");
        player = GameManager.instance.playerScript;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.isADS)
        {
            RadarW();
        }
        else
        {
            RadarN();
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
}
