using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarPulse : MonoBehaviour
{
    [Header("----- Radar Pulse -----")]
    [SerializeField] float radarSpeed;
    public float maxRange;

    private Transform pulseTf;
    private float curRange;

    private Collider[] colPing;
    
    // Start is called before the first frame update
    void Start()
    {
        pulseTf = transform.Find("/Player New/PlayerRadar/Pulse");
        maxRange = 40f;
    }

    // Update is called once per frame
    void Update()
    {
        if (curRange > maxRange)
        {
            curRange = 0f;
        }

        curRange += radarSpeed * Time.deltaTime;
        pulseTf.localScale = new Vector3(curRange, curRange);

        colPing = Physics.OverlapSphere(GameManager.instance.player.transform.position, curRange, 1<<20);

        foreach (Collider i in colPing)
        {
            /*
            if (i.gameObject.GetComponent<RadarPing>().isPinged == false)
            {
                Color tempColor = i.gameObject.GetComponent<SpriteRenderer>().color;
                tempColor.a = 1f;
                i.gameObject.GetComponent<SpriteRenderer>().color = tempColor;
                
                i.gameObject.GetComponent<RadarPing>().isPinged = true;
            }
            */

            RadarPing obj = i.gameObject.GetComponent<RadarPing>();

            if (obj.isPinged == false)
            {
                obj.isPinged = true;
            }
        }
    }
}
