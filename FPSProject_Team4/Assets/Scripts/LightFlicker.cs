using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] PointController reactor;
    
    private Color origColor;
    private Color eightColor;
    private Color sixColor;
    private Color fourColor;
    private Color twoColor;
    
    private float origIntensity;
    private float eightIntensity;
    private float sixIntensity;
    private float fourIntensity;
    private float twoIntensity;

    [Range(0,5)] private int lightState;
    
    private bool isBlinking;
    
    // Start is called before the first frame update
    void Start()
    {
        if (reactor == null)
        {
            reactor = transform.Find("Reactor_Core").gameObject.GetComponent<PointController>();
        }

        origColor = gameObject.GetComponent<Light>().color;
        eightColor = new Color(origColor.r - (origColor.r / 5), origColor.g - (origColor.g / 5), origColor.b - (origColor.b / 5));
        sixColor = new Color(eightColor.r - (origColor.r / 5), eightColor.g - (origColor.g / 5), eightColor.b - (origColor.b / 5));
        fourColor = new Color(sixColor.r - (origColor.r / 5), sixColor.g - (origColor.g / 5), sixColor.b - (origColor.b / 5));
        twoColor = new Color(fourColor.r - (origColor.r / 5), fourColor.g - (origColor.g / 5), fourColor.b - (origColor.b / 5));
        
        origIntensity = gameObject.GetComponent<Light>().intensity;
        eightIntensity = origIntensity - (origIntensity / 5);
        sixIntensity = eightIntensity - (origIntensity / 5);
        fourIntensity = sixIntensity - (origIntensity / 5);
        twoIntensity = fourIntensity - (origIntensity / 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (lightState != 5 && reactor.health == reactor.healthOrig)
        {
            gameObject.GetComponent<Light>().color = origColor;
            gameObject.GetComponent<Light>().intensity = origIntensity;
            lightState = 5;
        }
        else if (lightState != 4 && reactor.health < reactor.healthOrig && reactor.health >= reactor.healthOrig * 0.8f)
        {
            gameObject.GetComponent<Light>().color = eightColor;
            gameObject.GetComponent<Light>().intensity = eightIntensity;
            lightState = 4;
        }
        else if (lightState != 3 && reactor.health < reactor.healthOrig * 0.8f && reactor.health >= reactor.healthOrig * 0.6f)
        {
            gameObject.GetComponent<Light>().color = sixColor;
            gameObject.GetComponent<Light>().intensity = sixIntensity;
            lightState = 3;
        }
        else if (lightState != 2 && reactor.health < reactor.healthOrig * 0.6f && reactor.health >= reactor.healthOrig * 0.4f)
        {
            gameObject.GetComponent<Light>().color = fourColor;
            gameObject.GetComponent<Light>().intensity = fourIntensity;
            lightState = 2;
        }
        else if (lightState != 1 && reactor.health < reactor.healthOrig * 0.4f && reactor.health >= reactor.healthOrig * 0.2f)
        {
            gameObject.GetComponent<Light>().color = twoColor;
            gameObject.GetComponent<Light>().intensity = twoIntensity;
            lightState = 1;
        }
        else if (lightState != 0 && reactor.health < reactor.healthOrig * 0.2f)
        {
            gameObject.GetComponent<Light>().color = new Color(0, 0, 0, 0);
            gameObject.GetComponent<Light>().intensity = 0;
            lightState = 0;
        }
    }

    IEnumerator LightBlink()
    {
        isBlinking = true;

        Color tempColor = gameObject.GetComponent<Light>().color;

        yield return new WaitForSeconds(2);

        float start = Time.time;

        while (Time.time <= start + 1)
        {
            tempColor.r = tempColor.r - Mathf.Clamp01((Time.time - start) / 1);
            tempColor.g = tempColor.g - Mathf.Clamp01((Time.time - start) / 1);
            tempColor.b = tempColor.b - Mathf.Clamp01((Time.time - start) / 1);
            gameObject.GetComponent<Light>().color = tempColor;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator StartBlink()
    {
        StartCoroutine(LightBlink());

        yield return LightBlink();

        yield return new WaitForSeconds(1);

        isBlinking = false;
    }
    
    
}
