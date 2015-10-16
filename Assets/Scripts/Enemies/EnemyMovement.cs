using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    public Transform Player;
    public Transform Planet;
    public MovementBehaviour MovementType;
    public float VelocityImpulse = 5f;
    public float DetectionRange = 5f;
    public float OutRangeSlowdown = 1f;
    /// <summary>
    /// Prevents the collision capsule from coming out on the opposite side of the planet
    /// </summary>
    public float LimitCapsuleEnd = 0.95f;
    public bool UseSquareDistance = false;

    private Rigidbody _eRigidbody;
    private bool _isPlayerInRange;
    private float _distanceToPlayer;
    private int _playerLayer;

    public enum MovementBehaviour
    {
        Fixed,
        Constant,
        IncreaseOnDistance,
        DecreaseOnDistance,
    }

    void Awake()
    {
        _eRigidbody = GetComponent<Rigidbody>();
        _playerLayer = LayerMask.GetMask("PlayerLayer");
    }

    void Start()
    {
        InvokeRepeating("DetectPlayer", 0.0f, 0.65f);
    }

    void FixedUpdate()
    {
        if (_isPlayerInRange)
        {
            Vector3 forceIntensity = Vector3.zero;
            Vector3 targetDirection = (Player.position - transform.position).normalized;

            switch (MovementType)
            {
                case MovementBehaviour.Constant:
                    forceIntensity = VelocityImpulse * targetDirection; break;
                case MovementBehaviour.DecreaseOnDistance:
                    forceIntensity = VelocityImpulse * targetDirection / _distanceToPlayer; break;
                case MovementBehaviour.IncreaseOnDistance:
                    forceIntensity = VelocityImpulse * targetDirection * _distanceToPlayer; break;
            }

            // apply force on object rigidbody
            if(MovementType != MovementBehaviour.Fixed) { _eRigidbody.AddForce(forceIntensity); }
            else { _eRigidbody.velocity = (targetDirection * VelocityImpulse * Time.fixedDeltaTime); }
        }
        else // slowly slow down
        {
            _eRigidbody.velocity = Vector3.Lerp(_eRigidbody.velocity, Vector3.zero, Time.fixedDeltaTime * OutRangeSlowdown);
        }
    }

    void DetectPlayer()
    {
        float gPullerRadius = Mathf.Max(Planet.lossyScale.x, Planet.lossyScale.y, Planet.lossyScale.z) / 2f;
        float targetDistance = Mathf.Sqrt(gPullerRadius * gPullerRadius - DetectionRange * DetectionRange);

        Vector3 circleNormal = (transform.position - Planet.position).normalized;
        Vector3 capsuleOrigin = Planet.position - circleNormal * (gPullerRadius * LimitCapsuleEnd);
        Vector3 capsuleEnd = Planet.position + circleNormal * (targetDistance + DetectionRange);

        _isPlayerInRange = Physics.CheckCapsule(capsuleOrigin, capsuleEnd, DetectionRange, _playerLayer);
        _distanceToPlayer = _isPlayerInRange ? 
                                UseSquareDistance ? 
                                    Vector3.Distance(transform.position, Player.position) : 
                                    Vector3.SqrMagnitude(transform.position - Player.position) : 
                            Mathf.Infinity;
    }

    void OnDrawGizmosSelected()
    {
        // gravity opposite direction
        if (_eRigidbody != null)
        {
            Gizmos.color = Color.cyan;
            DrawArrow.ForGizmo(transform.position, _eRigidbody.velocity);
        }

        // bomb player detection range
        float planetRadius = Mathf.Max(Planet.lossyScale.x, Planet.lossyScale.y, Planet.lossyScale.z) / 2f;
        float targetDistance = Mathf.Sqrt(planetRadius * planetRadius - DetectionRange * DetectionRange);

        Vector3 circleNormal = (transform.position - Planet.position).normalized;
        Vector3 capsuleOrigin = Planet.position - circleNormal * (planetRadius * LimitCapsuleEnd);
        Vector3 capsuleEnd = Planet.position + circleNormal * (targetDistance + DetectionRange);

        DebugExtension.DrawCapsule(capsuleOrigin, capsuleEnd, _isPlayerInRange ? Color.red : Color.gray, DetectionRange);

        if (_isPlayerInRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Player.position - transform.position).normalized * _distanceToPlayer);
        }
    }
}
