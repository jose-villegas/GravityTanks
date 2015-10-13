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
    Transform mainCamera;
    Rigidbody playerRigidbody;

    bool isWalking = false;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float f = Input.GetAxisRaw("Jump");

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

        mainCamera = Camera.main.transform;

        movement = mainCamera.right * h + mainCamera.up * v;
        movement = movement.normalized * movementSpeed * Time.fixedDeltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turn(float h, float v)
    {
        turnDirection = isWalking ? new Vector3(h, 0f, v) : turnDirection;
        upToCenterG = (transform.position - gravityPuller.position).normalized;

        Quaternion surfaceRot = Quaternion.FromToRotation(Vector3.up, upToCenterG);
        Quaternion rotAround = Quaternion.FromToRotation(Vector3.right, turnDirection);
        // smooth rotation with damping parameter
        playerRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, surfaceRot * rotAround, damping * Time.deltaTime));
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
        DrawArrow.ForGizmo(transform.position + movement * maxScale, turnDirection);

        // gravity opposite direction
        Gizmos.color = Color.cyan;
        DrawArrow.ForGizmo(transform.position + upToCenterG * maxScale, upToCenterG);
    }
}
