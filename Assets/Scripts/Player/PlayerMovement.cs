using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    public float movementSpeed = 5f;
    public float damping = 8f;
    public Transform gravityPuller;

    Vector3 movement;
    Vector3 turnAround;
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
        // flags
        isWalking = h != 0 || v != 0;
        // player movement calls
        Move(h, v);
        Turn(h, v);
    }

    void Move(float h, float v)
    {
        movement = transform.right * h + transform.forward * v;
        movement = movement.normalized * movementSpeed * Time.fixedDeltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turn(float h, float v)
    {
        Vector3 upToCenterG = (transform.position - gravityPuller.position).normalized;
        transform.up = Vector3.Slerp(transform.up, upToCenterG, damping * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        // velocity vector direction
        if (playerRigidbody != null && playerRigidbody.velocity.magnitude > 0)
        {
            DrawArrow.ForGizmo(transform.position, playerRigidbody.velocity.normalized);
        }
    }
}
