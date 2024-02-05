using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Waypoint : MonoBehaviour
{
    [SerializeField] public Image Img;
    [SerializeField] public Sprite warnSprite;
    [SerializeField] public Sprite errorSprite;
    [SerializeField] public Transform target;
    [SerializeField] TMP_Text distance;

    


    public Waypoint(Transform tgt)
    {
        target = tgt;
    }
    private void Update()
    {
        if (target != null)
        {
            float minX = Img.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;

            float minY = Img.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minY;

            Vector2 pos = Camera.main.WorldToScreenPoint(target.position);
            Vector3 toTarget = (target.position) - GameManager.instance.player.transform.position;

            if (Vector3.Angle(toTarget, Camera.main.transform.forward) >= 90)
            {
                if (pos.x < (Screen.width / 2) - Img.GetPixelAdjustedRect().width)
                {
                    pos.x = maxX;
                }
                else
                {
                    pos.x = minX;
                }
                pos.y = maxY - pos.y;
            }

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            Img.transform.position = pos;

            distance.text = Vector3.Distance(target.position, Camera.main.transform.position).ToString("0") + "m";
        }
    }


}
