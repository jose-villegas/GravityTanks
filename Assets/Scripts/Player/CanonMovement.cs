using UnityEngine;
using System.Collections;

public class CanonMovement : MonoBehaviour {

    public Transform RotateAround;
    public float CanonDistance = 0.5f;
    public float TurningSpeed = 8.0f;
    public float CamRayLength = 100f;

    public int InputMask { get; private set; }

    void Start ()
    {
        InputMask = LayerMask.GetMask("InputCapture");
    }

    // Update is called once per frame
    void Update ()
    {
        Move();
    }

    void Move()
    {
        // set cannon to proper distance to the player or target
        transform.position = RotateAround.position + transform.forward * CanonDistance;

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit inputPlaneHit;

        if (Physics.Raycast(camRay, out inputPlaneHit, CamRayLength, InputMask))
        {
            Vector3 mouseDirection = (inputPlaneHit.point - transform.position).normalized;
            Quaternion toMouse = Quaternion.LookRotation(mouseDirection, RotateAround.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toMouse, TurningSpeed * Time.deltaTime);
        }
    }
}
