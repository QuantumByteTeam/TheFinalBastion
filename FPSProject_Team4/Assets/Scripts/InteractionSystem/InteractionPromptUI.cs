using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        mainCam= Camera.main;



    }

    private void LateUpdate()
    {
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position +rotation*Vector3.forward, rotation * Vector3.up);

    }
}
