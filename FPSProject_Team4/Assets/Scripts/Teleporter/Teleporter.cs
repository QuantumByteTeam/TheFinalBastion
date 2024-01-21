using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("----- Teleporter -----")]
    [SerializeField] Transform destination;
    // [SerializeField] GameObject destination;
    [SerializeField] int cooldown;

    private GameObject desObject;
    private GameObject player;
    private Transform playerPos;

    private bool onCooldown;
    
    // Start is called before the first frame update
    void Start()
    {
        desObject = destination.parent.gameObject;
        player = GameManager.instance.player;
        playerPos = GameManager.instance.player.transform;
    }

    // Update is called once per frame
    void Update() { }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!onCooldown)
            {
                StartCoroutine(TeleporterCD());
            }
        }
    }

    IEnumerator TeleporterCD()
    {
        onCooldown = true;
        
        player.SetActive(false);
        playerPos.position = destination.position;
        desObject.GetComponent<Teleporter>().onCooldown = true;
        player.SetActive(true);

        yield return new WaitForSeconds(cooldown);

        desObject.GetComponent<Teleporter>().onCooldown = false;
        onCooldown = false;
    }
}
