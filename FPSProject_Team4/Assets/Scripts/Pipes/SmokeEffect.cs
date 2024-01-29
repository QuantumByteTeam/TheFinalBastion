using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    [Header("----- Smoke -----")]
    [SerializeField] float spreadSpeed;
    [SerializeField] float maxRange;

    private GameObject parent;
    private ParticleSystem smoke;
    private BoxCollider boxCol;

    private float curRange;

    private bool stopScaling;
    private bool smokeFade;
    private bool stopUnscale;
    
    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        smoke = gameObject.GetComponent<ParticleSystem>();
        boxCol = gameObject.GetComponent<BoxCollider>();

        if (spreadSpeed == 0)
        {
            spreadSpeed = 0.15f;
        }

        if (maxRange == 0)
        {
            maxRange = 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!smokeFade)
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
            ScaleSmoke();
        }

        if (parent.GetComponent<PipeSystem>().fullHealth)
        {
            StartCoroutine(DestroySelf());
        }

        if (smokeFade && !stopUnscale)
        {
            FadeSmoke();
        }
    }

    void ScaleSmoke()
    {
        curRange += spreadSpeed * Time.deltaTime;
        transform.localScale = new Vector3(curRange, curRange, curRange);
        boxCol.transform.localScale = new Vector3(curRange, curRange, curRange);
    }

    void FadeSmoke()
    {
        curRange -= (spreadSpeed - 0.1f) * Time.deltaTime;
        transform.localScale = new Vector3(curRange, curRange, curRange);
        boxCol.transform.localScale = new Vector3(curRange, curRange, curRange);
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(3);

        stopScaling = true;
        smokeFade = true;

        yield return new WaitUntil(() => stopUnscale);

        parent.GetComponent<PipeSystem>().effectCreated = false;

        Destroy(gameObject);
    }

    IEnumerator VignetteEnter()
    {
        UIManager.instance.smokeVignette.SetActive(true);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator VignetteExit()
    {
        yield return new WaitForSeconds(3);
        
        UIManager.instance.smokeVignette.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            StartCoroutine(VignetteEnter());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            StartCoroutine(VignetteExit());
        }
    }
}
