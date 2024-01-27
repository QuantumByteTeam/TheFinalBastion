using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Teleporter : MonoBehaviour, ISimpleInteractable
{
    [Header("----- Teleporter -----")]
    [Range(0, 9)] public int teleId;
    public string teleName;
    [SerializeField] int cooldown;
    
    [Header("----- Bools -----")]
    [SerializeField] bool interactable;
    [SerializeField] bool onlyReceive;
    public bool onCooldown;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource audIdle;
    [Range(0, 1)][SerializeField] float audIdleVol;
    [SerializeField] AudioSource audUse;
    [Range(0, 1)][SerializeField] float audUseVol;

    [Header("----- Collider -----")]
    [SerializeField] Collider ignoreCol;

    [Header("----- Materials -----")]
    [SerializeField] private Material idZero;
    [SerializeField] private Material idOne;
    [SerializeField] private Material idTwo;
    [SerializeField] private Material idThree;
    [SerializeField] private Material idFour;
    [SerializeField] private Material idFive;
    
    [Header("----- Interactable -----")]
    [SerializeField] string prompt;
    
    private List<GameObject> teleporters = new List<GameObject>();
    // private List<Transform> destinations = new List<Transform>();
    
    private GameObject player;
    private Transform playerPos;
    private ParticleSystem partSys;

    private int maxId;
    private float timer;
    private bool justTeleported;
    private bool updateTele;
    private bool isPlayingIdle;
    private bool isPlayingBool;
    public string InteractionPrompt => prompt;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;
        playerPos = player.transform;
        partSys = gameObject.GetComponent<ParticleSystem>();
        
        Physics.IgnoreCollision(player.GetComponent<Collider>(), ignoreCol, true);

        audIdle.volume = audIdleVol;
        audUse.volume = audUseVol;

        maxId = 0;

        UpdateTeleTex();

        if (interactable)
        {
            gameObject.layer = 7;
        }
        
        if (!onlyReceive && !interactable)
        {
            FindTeleporter();
        }
        else if (!onlyReceive & interactable)
        {
            FindTeleporter();
            
            string destName = teleporters.First().GetComponent<Teleporter>().name;

            prompt = "Teleport to " + teleName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!onCooldown && !audIdle.isPlaying)
        {
            audIdle.Play();
        }

        if (updateTele)
        {
            UpdateTeleTex();
            
            FindTeleporter();

            string destName = teleporters.First().GetComponent<Teleporter>().name;

            prompt = "Teleport to " + teleName;

            updateTele = false;
        }

        if (teleporters.Count == 0)
        {
            partSys.Stop();
        }
        
        if (justTeleported)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
        }

        if (timer > 10)
        {
            justTeleported = false;
        }
    }

    public void SimpleInteract(SimpleInteractor simpleInteractor)
    {
        if (interactable)
        {
            ChangeId();
        }
    }

    void ChangeId()
    {
        Debug.Log("Click Clack");
        
        if (teleId >= 0 && teleId < maxId)
        {
            teleId++;
        }
        else
        {
            teleId = 0;
        }

        updateTele = true;
    }

    void FindTeleporter()
    {
        GameObject[] teleObj = GameObject.FindGameObjectsWithTag("Teleporter");
        
        GetMaxId(teleObj);
        
        teleporters.Clear();

        foreach (GameObject i in teleObj)
        {
            Teleporter teleScript = i.GetComponent<Teleporter>();

            if (i.transform.GetInstanceID() != this.transform.GetInstanceID())
            {
                if (teleScript.teleId == teleId)
                {
                    teleporters.Add(i);
                }
            }
        }
    }

    void GetMaxId(GameObject[] objArray)
    {
        foreach (GameObject i in objArray)
        {
            int id = i.GetComponent<Teleporter>().teleId;

            if (id > maxId)
            {
                maxId = id;
            }
        }
    }

    void UpdateTeleTex()
    {
        var partSysMain = partSys.main;
        
        switch (teleId)
        {
            case 0:
                gameObject.GetComponent<MeshRenderer>().material = idZero;
                partSysMain.startColor = new Color(0, 0.8313f, 1, 1);
                break;
            case 1:
                gameObject.GetComponent<MeshRenderer>().material = idOne;
                partSysMain.startColor = new Color(1, 0, 0, 1);
                break;
            case 2:
                gameObject.GetComponent<MeshRenderer>().material = idTwo;
                partSysMain.startColor = new Color(0.73272f, 0, 1, 1);
                break;
            case 3:
                gameObject.GetComponent<MeshRenderer>().material = idThree;
                partSysMain.startColor = new Color(0, 1, 0.2549f, 1);
                break;
            case 4:
                gameObject.GetComponent<MeshRenderer>().material = idFour;
                partSysMain.startColor = new Color(1, 0.8470f, 0, 1);
                break;
            default:
                gameObject.GetComponent<MeshRenderer>().material = idFive;
                partSysMain.startColor = new Color(236/255f, 64/255f, 122/255f, 1);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && teleporters.Count > 0)
        {
            if (!justTeleported && !onCooldown)
            {
                StartCoroutine(Teleport());
            }
        }
    }

    IEnumerator Teleport()
    {
        onCooldown = true;
        partSys.Stop();
        partSys.Clear();
        audIdle.Stop();
        
        player.SetActive(false);
        
        int rand = Random.Range(0, teleporters.Count);

        playerPos.position = teleporters[rand].transform.position;
        audUse.Play();

        if (!justTeleported)
        {
            justTeleported = true;
        }
        
        player.SetActive(true);

        StartCoroutine(Cooldown());
        
        yield return Cooldown();
        
        partSys.Play();
        onCooldown = false;
    }

    IEnumerator Cooldown()
    {
        foreach (GameObject i in teleporters)
        {
            i.GetComponent<Teleporter>().onCooldown = true;
            i.GetComponent<Teleporter>().audIdle.Stop();
            i.GetComponent<ParticleSystem>().Stop();
            i.GetComponent<ParticleSystem>().Clear();
        }

        yield return new WaitForSeconds(cooldown);

        foreach (GameObject i in teleporters)
        {
            i.GetComponent<ParticleSystem>().Play();
            i.GetComponent<Teleporter>().onCooldown = false;
        }
    }
}
