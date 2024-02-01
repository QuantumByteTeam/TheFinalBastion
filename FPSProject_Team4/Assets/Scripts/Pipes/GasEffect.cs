using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Image = UnityEngine.UI.Image;

public class GasEffect : MonoBehaviour
{
    [Header("----- Gas -----")]
    [SerializeField] float spreadSpeed;
    [SerializeField] float maxRange;
    [SerializeField] float gasDamage;
    [SerializeField] float gasDamageInterval;

    private GameObject parent;
    private ParticleSystem gas;
    private BoxCollider boxCol;

    private float curRange;

    private bool stopScaling;
    private bool gasFade;
    private bool stopUnscale;

    private bool hasDmg;

    private GameObject vignette;
    private float vignetteAlpha;
    private float darkAlpha;
    private bool vignFadeIn;
    private bool vignFadeOut;
    private bool isVignEnterRunning;
    private bool isVignExitRunning;
    private int timer;

    private RadarPulse pulse;
    
    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        gas = gameObject.GetComponent<ParticleSystem>();
        boxCol = gameObject.GetComponent<BoxCollider>();
        vignette = UIManager.instance.smokeVignette;
        pulse = GameObject.Find("PlayerRadar").GetComponent<RadarPulse>();

        if (spreadSpeed == 0)
        {
            spreadSpeed = 0.15f;
        }

        if (maxRange == 0)
        {
            maxRange = 1f;
        }

        if (gasDamage == 0)
        {
            gasDamage = 10;
        }

        if (gasDamageInterval == 0)
        {
            gasDamageInterval = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gasFade)
        {
            if (curRange > maxRange)
            {
                stopScaling = true;
            }
        }
        else
        {
            if (curRange <= 0)
            {
                stopUnscale = true;
            }
        }

        if (!stopScaling)
        {
            ScaleGas();
        }

        if (parent.GetComponent<PipeSystem>().fullHealth)
        {
            StartCoroutine(DestroySelf());
        }

        if (gasFade && !stopUnscale)
        {
            FadeGas();
        }
    }

    void ScaleGas()
    {
        curRange += spreadSpeed * Time.deltaTime;
        transform.localScale = new Vector3(curRange, curRange, curRange);
        boxCol.transform.localScale = new Vector3(curRange, curRange, curRange);
    }

    void FadeGas()
    {
        curRange -= (spreadSpeed - 0.1f) * Time.deltaTime;
        transform.localScale = new Vector3(curRange, curRange, curRange);
        boxCol.transform.localScale = new Vector3(curRange, curRange, curRange);
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(3);

        stopScaling = true;
        gasFade = true;

        yield return new WaitUntil(() => stopUnscale);

        parent.GetComponent<PipeSystem>().effectCreated = false;

        Destroy(gameObject);
    }

    IEnumerator VignetteEnter()
    {
        if (isVignExitRunning)
        {
            StopCoroutine(VignetteExit());
            isVignExitRunning = false;
        }
        
        isVignEnterRunning = true;
        
        timer = 0;
        
        Color vign = vignette.GetComponent<Image>().color;
        Color dark = vignette.transform.GetChild(0).gameObject.GetComponent<Image>().color;

        vign = new Color(0f, 0.1f, 0f, 0);
        dark = new Color(0f, 0.1f, 0f, 0);

        yield return new WaitUntil(() => timer == 2);
        
        vignette.SetActive(true);

        // vignFadeOut = false;
        // vignFadeIn = true;

        float start = Time.time;

        //while (vignFadeIn && !vignFadeOut && vign.a <= 0.5f)
        while (isVignEnterRunning && vign.a <= 0.5f)
        {
            float vignFloat = 0f + Mathf.Clamp01((Time.time - start) * 0.2f);
            vign.a = vignFloat;
            vignette.GetComponent<Image>().color = vign;
            dark.a = vignFloat / 1.1f;
            vignette.transform.GetChild(0).gameObject.GetComponent<Image>().color = dark;

            yield return new WaitForEndOfFrame();
        }
        
        isVignEnterRunning = false;
    }

    IEnumerator VignetteExit()
    {
        if (isVignEnterRunning)
        {
            StopCoroutine(VignetteEnter());
            isVignEnterRunning = false;
        }
        
        isVignExitRunning = true;
        
        timer = 0;
        
        Color vign = vignette.GetComponent<Image>().color;
        Color dark = vignette.transform.GetChild(0).gameObject.GetComponent<Image>().color;
        
        // vignFadeIn = false;
        // vignFadeOut = true;
        
        yield return new WaitForSeconds(5);

        float start = Time.time;
        
        //while (vignFadeOut && !vignFadeIn && vign.a >= 0)
        while (isVignExitRunning && vign.a >= 0)
        {
            float vignFloat = vign.a - Mathf.Clamp01((Time.time - start) * 0.001f);
            vign.a = vignFloat;
            vignette.GetComponent<Image>().color = vign;
            float darkFloat = vign.a / 1.1f;
            dark.a = darkFloat;
            vignette.transform.GetChild(0).gameObject.GetComponent<Image>().color = dark;

            yield return new WaitForEndOfFrame();
        }
        
        isVignExitRunning = false;
    }
    
    IEnumerator EnemyEnter(Collider col)
    {
        Debug.Log("Enemy entered Gas");
        col.GetComponent<NavMeshAgent>().speed /= 2f;
        col.GetComponent<EnemyAI>().isSmokeBlind = true;

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator EnemyExit(Collider col)
    {
        Debug.Log("Enemy exited Gas");
        col.GetComponent<NavMeshAgent>().speed *= 2f;
        col.GetComponent<EnemyAI>().isSmokeBlind = false;

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator GasDamage(Collider col)
    {
        hasDmg = true;

        IDamageable dmg = col.GetComponent<IDamageable>();
        
        dmg.takeDamage(gasDamage, 0);

        yield return new WaitForSeconds(gasDamageInterval);

        hasDmg = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            pulse.maxRange /= 8f;
            StartCoroutine(VignetteEnter());
        }
        else if (other.CompareTag("Enemy") && !other.isTrigger)
        {
            StartCoroutine(EnemyEnter(other));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (timer < 2)
        {
            timer++;
        }
        
        if ((other.CompareTag("Player") || other.CompareTag("Enemy")) && !other.isTrigger)
        {
            if (hasDmg == false)
            {
                StartCoroutine(GasDamage(other));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            pulse.maxRange *= 8;
            StartCoroutine(VignetteExit());
        }
        
        if (other.CompareTag("Enemy") && !other.isTrigger)
        {
            StartCoroutine(EnemyExit(other));
        }
    }
}
