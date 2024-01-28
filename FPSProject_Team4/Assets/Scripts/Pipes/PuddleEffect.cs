using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PuddleEffect : MonoBehaviour
{
    [Header("----- Puddle -----")]
    [SerializeField] float spreadSpeed;
    [SerializeField] float maxRange;
    
    private GameObject parent;
    private GameObject ground;
    private SpriteRenderer puddle;
    private BoxCollider boxCol;

    private float curRange;
    private float oldZpos;
    
    private bool isGrounded;
    private bool isVisible;
    private bool stopScaling;
    private bool evaporate;
    private bool stopUnscale;
    
    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        puddle = gameObject.GetComponent<SpriteRenderer>();
        boxCol = gameObject.GetComponent<BoxCollider>();
        // gameObject.GetComponent<Rigidbody>().isKinematic = false;

        if (spreadSpeed == 0)
        {
            spreadSpeed = 0.15f;
        }

        if (maxRange == 0)
        {
            maxRange = 4.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded && !isVisible)
        {
            GroundedStartup();
        }

        if (!evaporate)
        {
            if (isVisible && curRange > maxRange)
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
        
        if (isVisible && !stopScaling)
        {
            ScalePuddle();
        }

        if (parent.GetComponent<PipeSystem>().fullHealth)
        {
            StartCoroutine(DestroySelf());
        }

        if (evaporate && !stopUnscale)
        {
            EvaporatePuddle();
        }
    }

    void GroundedStartup()
    {
        StartCoroutine(ChangeZ());
        
        isVisible = true;
        puddle.enabled = true;
        boxCol.isTrigger = true;
    }
    
    void ScalePuddle()
    {
        curRange += spreadSpeed * Time.deltaTime;
        transform.localScale = new Vector3(curRange, curRange, 0.1f);
    }

    void EvaporatePuddle()
    {
        curRange -= (spreadSpeed - 0.1f) * Time.deltaTime;
        transform.localScale = new Vector3(curRange, curRange, 0.1f);
    }

    IEnumerator ChangeZ()
    {
        Vector3 newPos = gameObject.transform.position;
        newPos.y += 0.10f;
        gameObject.transform.position = newPos;
        
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(3);
        
        stopScaling = true;
        evaporate = true;

        yield return new WaitUntil(() => stopUnscale);

        parent.GetComponent<PipeSystem>().effectCreated = false;
        
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        isGrounded = true;

        ground = other.gameObject;

        if (ground.GetComponent<Renderer>().bounds.size.x < maxRange)
        {
            maxRange = ground.GetComponent<Renderer>().bounds.size.x - 1;
        }
        
        Destroy(gameObject.GetComponent<Rigidbody>());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entering...");
        
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            GameManager.instance.playerScript.PlayerSpeed *= 5f;
        }
        else if (other.CompareTag("Enemy") && !other.isTrigger)
        {
            NavMeshAgent enemy = other.GetComponent<NavMeshAgent>();
            enemy.speed /= 2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exiting...");
        
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            GameManager.instance.playerScript.PlayerSpeed /= 5f;
        }
        else if (other.CompareTag("Enemy") && !other.isTrigger)
        {
            NavMeshAgent enemy = other.GetComponent<NavMeshAgent>();
            enemy.speed = enemy.GetComponent<EnemyAI>().origSpeed;
        }
    }
}
