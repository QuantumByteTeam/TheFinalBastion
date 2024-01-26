using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RadarPing : MonoBehaviour
{
    [Header("----- Is Pinged? -----")]
    public bool isPinged;

    [Header("----- Ping Type -----")]
    [SerializeField] bool isPlayer;
    [SerializeField] bool isEnemy;
    [SerializeField] bool isPoint;
    [SerializeField] bool isWeapon;
    [SerializeField] bool isItem;
    [SerializeField] bool isTurret;

    private SpriteRenderer icon;
    
    private Color playerColor;
    private Color enemyColor;
    private Color pointColor;
    private Color weaponColor;
    private Color itemColor;
    private Color turretColor;

    private bool isBlinking;
    private float fadeDuration = 1;
    
    void Awake()
    {
        playerColor = new Color(56f/255f, 142f/255f, 60f/255f, 0f);
        enemyColor = new Color(244f/255f, 67f/255f, 54f/255f, 0f);
        pointColor = new Color(33f/255f, 150f/255f, 243f/255f, 0f);
        weaponColor = new Color(255f/255f, 152f/255f, 0f/255f, 0f);
        itemColor = new Color(224f/255f, 224f/255f, 224f/255f, 0f);
        turretColor = new Color(76f/255f, 175f/255f, 80f/255f, 0f);

        icon = gameObject.GetComponent<SpriteRenderer>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (isPlayer)
        {
            icon.color = playerColor;
        }
        else if (isEnemy)
        {
            icon.color = enemyColor;
        }
        else if (isPoint)
        {
            icon.color = pointColor;
        }
        else if (isWeapon)
        {
            icon.color = weaponColor;
        }
        else if (isItem)
        {
            icon.color = itemColor;
        }
        else if (isTurret)
        {
            icon.color = turretColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isActivePaused)
        {
            if (!GameManager.instance.isPaused)
            {
                /*
                if (isPinged)
                {
                    StartCoroutine(PingFade());
                }
                */

                if (isPlayer)
                {
                    StartCoroutine(PlayerIcon());
                }
                else if (isPoint)
                {
                    PointController point = this.transform.parent.gameObject.GetComponent<PointController>();
                    
                    Color tempColor = icon.color;
                    tempColor.a = 1f;
                    icon.color = tempColor;

                    if (point.isAttacked && !isBlinking)
                    {
                        StartCoroutine(PointBlink(point));
                    }
                }
                
                if (isPinged && isEnemy)
                {
                    StartCoroutine(EnemyPing());
                }
                else if (isPinged && isTurret)
                {
                    StartCoroutine(TurretPing());
                }
                else if (isPinged && (isWeapon || isItem))
                {
                    if (!isBlinking)
                    {
                        StartCoroutine(ItemPing());
                    }
                }
            }
        }
    }

    /*
    IEnumerator PingFade()
    {
        while (icon.color.a > 0)
        {
            icon.color -= new Color(0, 0, 0, 1f * Time.deltaTime);
            
            yield return new WaitForSeconds(1f);
        }

        if (icon.color.a <= 0)
        {
            yield return new WaitForSeconds(2f);
            isPinged = false;
        }
    }
    */

    IEnumerator PlayerIcon()
    {
        PlayerController player = GameManager.instance.playerScript;

        if (player.IsShooting)
        {
            Color tempColor = icon.color;
            tempColor.a = 1f;
            icon.color = tempColor;

            yield return new WaitUntil(() => !player.IsShooting);

            float start = Time.time;

            while (Time.time <= start + fadeDuration)
            {
                tempColor.a = 1f - Mathf.Clamp01((Time.time - start) / fadeDuration);
                icon.color = tempColor;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator EnemyIcon()
    {
        EnemyAI enemy = this.transform.parent.gameObject.GetComponent<EnemyAI>();

        if (enemy.isRoaming || enemy.isShooting)
        {
            Color tempColor = icon.color;
            tempColor.a = 1f;
            icon.color = tempColor;

            yield return new WaitUntil(() => !enemy.isRoaming && !enemy.isShooting);

            float start = Time.time;
            
            while (Time.time <= start + fadeDuration)
            {
                tempColor.a = 1f - Mathf.Clamp01((Time.time - start) / fadeDuration);
                icon.color = tempColor;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator EnemyPing()
    {
        StartCoroutine(EnemyIcon());

        yield return EnemyIcon();

        isPinged = false;
    }

    IEnumerator TurretIcon()
    {
        turret turret = this.transform.parent.gameObject.GetComponent<turret>();

        if (turret.isShooting)
        {
            Color tempColor = icon.color;
            tempColor.a = 1f;
            icon.color = tempColor;

            yield return new WaitUntil(() => !turret.isShooting);

            float start = Time.time;
            
            while (Time.time <= start + fadeDuration)
            {
                tempColor.a = 1f - Mathf.Clamp01((Time.time - start) / fadeDuration);
                icon.color = tempColor;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator TurretPing()
    {
        StartCoroutine(TurretIcon());

        yield return TurretIcon();

        isPinged = false;
    }

    IEnumerator PointBlink(PointController pc)
    {
        isBlinking = true;
        
        while (pc.isAttacked)
        {
            icon.enabled = true;
            yield return new WaitForSeconds(0.5f);
            icon.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }
        icon.enabled = true;

        yield return new WaitUntil(() => !pc.isAttacked);

        isBlinking = false;
    }

    IEnumerator ItemIcon()
    {
        isBlinking = true;
        
        Color tempColor = icon.color;
        tempColor.a = 1f;
        icon.color = tempColor;

        yield return new WaitForSeconds(3);
        
        float start = Time.time;

        while (Time.time <= start + fadeDuration)
        {
            tempColor.a = 1f - Mathf.Clamp01((Time.time - start) / fadeDuration);
            icon.color = tempColor;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ItemPing()
    {
        StartCoroutine(ItemIcon());

        yield return ItemIcon();

        yield return new WaitForSeconds(2);

        isPinged = false;
        isBlinking = false;
    }
    
}
