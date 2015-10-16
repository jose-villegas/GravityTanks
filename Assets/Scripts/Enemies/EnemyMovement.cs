using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour {

    public Transform Player;
    public Transform Planet;
    public float VelocityImpulse = 5f;
    public float DetectionRange = 5f;
    public float OutRangeSlowdown = 1f;
    /// <summary>
    /// Prevents the collision capsule from coming out on the opposite side of the planet
    /// </summary>
    public float LimitCapsuleEnd = 0.95f;

    private Rigidbody _eRigidbody;
    private bool _isPlayerInRange;
    private int _playerLayer;

    void Awake()
    {
        _eRigidbody = GetComponent<Rigidbody>();
        _playerLayer = LayerMask.GetMask("PlayerLayer");
    }

    void Start()
    {
        InvokeRepeating("DetectPlayer", 0.0f, 0.5f);
    }

    void FixedUpdate()
    {
        if (_isPlayerInRange)
            _eRigidbody.velocity = (Player.position - transform.position).normalized * VelocityImpulse * Time.fixedDeltaTime;
        else
            _eRigidbody.velocity = Vector3.Lerp(_eRigidbody.velocity, Vector3.zero, Time.fixedDeltaTime * OutRangeSlowdown); // slowly slow down
    }

    void DetectPlayer()
    {
        float gPullerRadius = Mathf.Max(Planet.lossyScale.x, Planet.lossyScale.y, Planet.lossyScale.z) / 2f;
        float targetDistance = Mathf.Sqrt(gPullerRadius * gPullerRadius - DetectionRange * DetectionRange);

        Vector3 circleNormal = (transform.position - Planet.position).normalized;
        Vector3 capsuleOrigin = Planet.position - circleNormal * (gPullerRadius * LimitCapsuleEnd);
        Vector3 capsuleEnd = Planet.position + circleNormal * (targetDistance + DetectionRange);

        _isPlayerInRange = Physics.CheckCapsule(capsuleOrigin, capsuleEnd, DetectionRange, _playerLayer);
    }

    void OnDrawGizmosSelected()
    {
        // gravity opposite direction
        if (_eRigidbody != null) { 
            Gizmos.color = Color.cyan;
            DrawArrow.ForGizmo(transform.position, _eRigidbody.velocity);
        }

        // bomb player detection range
        float gPullerRadius = Mathf.Max(Planet.lossyScale.x, Planet.lossyScale.y, Planet.lossyScale.z) / 2f;
        float targetDistance = Mathf.Sqrt(gPullerRadius * gPullerRadius - DetectionRange * DetectionRange);

        Vector3 circleNormal = (transform.position - Planet.position).normalized;
        Vector3 capsuleOrigin = Planet.position - circleNormal * (gPullerRadius * LimitCapsuleEnd);
        Vector3 capsuleEnd = Planet.position + circleNormal * (targetDistance + DetectionRange);

        DebugExtension.DrawCapsule(capsuleOrigin, capsuleEnd, _isPlayerInRange ? Color.red : Color.gray, DetectionRange);
    }
}
