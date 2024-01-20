using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool invertY;

    float xRot;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //makes sure u dont flick out of the tab 
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isActivePaused) //checks if game is Actve paused (interaction menus), if paused it doesnt call anything below
        { 
          //get input
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

            if (invertY) //inverts Y axis controls
                xRot += mouseY;
            else
                xRot -= mouseY;



            //clamps rotation on the X-Axis
            xRot = Mathf.Clamp(xRot, lockVertMin, lockVertMax);

            //rotates camera on the X-axis
            transform.localRotation = Quaternion.Euler(xRot, 0, 0);

            //rotates player on the Y-Axis
            transform.parent.Rotate(Vector3.up * mouseX);

        }


    }
}
