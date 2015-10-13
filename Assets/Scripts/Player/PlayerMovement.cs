using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float movementSpeed = 5f;
    public float damping = 8f;
    public float jumpStrength = 2f;
    public Transform gravityPuller;
    public Transform playerStick;

    Vector3 movement;
    Vector3 turnDirection;
    Vector3 upToCenterG;
    Rigidbody playerRigidbody;

    bool isWalking = false;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float f = Input.GetAxis("Jump");

        // flags
        isWalking = h != 0 || v != 0;
        // player movement calls
        Turn(h, v);
        Move(h, v);
        Jump(f);
    }

    void Move(float h, float v)
    {
        if (!isWalking) return;

        Transform mainCamera = Camera.main.transform;

        movement = mainCamera.right * h + mainCamera.up * v;
        movement = movement.normalized * movementSpeed * Time.fixedDeltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turn(float h, float v)
    {
        upToCenterG = (transform.position - gravityPuller.position).normalized;

        // smooth rotation with damping parameter
        if (isWalking)
        {
            Transform mainCamera = Camera.main.transform;

            turnDirection = Quaternion.LookRotation(mainCamera.up, upToCenterG) * new Vector3(h, 0f, v);
            Quaternion rotAround = Quaternion.LookRotation(turnDirection, upToCenterG);
            // look forward to input plane direction
            playerRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, rotAround, damping * Time.deltaTime));
        }
        else
        {
            Quaternion rotAround = Quaternion.LookRotation(turnDirection, upToCenterG);
            // rot to up vector based on gravity
            playerRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, rotAround, damping * Time.deltaTime));
        }
    }

    void Jump(float f)
    {
        if (f <= 0f) return;

        playerRigidbody.velocity = transform.up * f * jumpStrength;
    }

    void OnDrawGizmosSelected()
    {
        if(Application.isPlaying)
        {
            // velocity vector direction
            Gizmos.color = Color.magenta;
            DrawArrow.ForGizmo(transform.position, playerRigidbody.velocity);
        }
        
        // draw initial velocity vector
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        // movement direction
        Gizmos.color = Color.black;
        DrawArrow.ForGizmo(transform.position + movement * maxScale, movement);

        // gravity opposite direction
        Gizmos.color = Color.cyan;
        DrawArrow.ForGizmo(transform.position + upToCenterG * maxScale, upToCenterG);

        // axis input plane
        Gizmos.color = Color.red;
        DebugExtension.DrawCircle(transform.position, upToCenterG, Color.red, 1f);
        // draw current input position
        Gizmos.DrawCube(transform.position + turnDirection, Vector3.one * 0.15f);
    }
}
