﻿using UnityEngine;
using System.Collections;

public class CanonMovement : MonoBehaviour {

    public Transform rotateAround;
    public float canonDistance = 0.5f;

    int inputMask;
    float camRayLength = 100f;

    // Use this for initialization
    void Start ()
    {
        inputMask = LayerMask.GetMask("InputCapture");
    }

    // Update is called once per frame
    void Update ()
    {
        Move();
    }

    void Move()
    {
        // set cannon to proper distance to the player or target
        transform.position = rotateAround.position + transform.forward * canonDistance;

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit inputPlaneHit;

        if (Physics.Raycast(camRay, out inputPlaneHit, camRayLength, inputMask))
        {
            Vector3 playerToMouse = (inputPlaneHit.point - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(playerToMouse, rotateAround.up);
        }
    }
}
