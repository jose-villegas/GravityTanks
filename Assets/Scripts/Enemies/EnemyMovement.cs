using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    #region MovementBehaviour enum

    public enum MovementBehaviour
    {
        Fixed,
        Constant,
        IncreaseOnDistance,
        DecreaseOnDistance
    }

    #endregion

    private float distanceToPlayer;

    private Rigidbody eRigidbody;
    private bool isPlayerInRange;
    private int playerLayer;

    /// <summary>
    ///     Traslates the area detection capsule beginning position
    ///     from the object position, aligned with Planet center of mass
    /// </summary>
    public Vector3 CapsuleBegin = Vector3.zero;

    /// <summary>
    ///     Translates the area detection capsule ending position
    ///     from the object position, aligned with Planet center of mass
    /// </summary>
    public Vector3 CapsuleEnd = Vector3.zero;

    public float CapsuleRadius = 1f;

    public MovementBehaviour MovementType;
    public float OutRangeSlowdown = 1f;

    /// <summary>
    ///     Planetary body where this object resides
    /// </summary>
    public Transform Planet;

    public Transform Player;
    public bool UseSquareDistance = false;
    public float VelocityImpulse = 5f;

    private void Awake()
    {
        eRigidbody = GetComponent<Rigidbody>();
        playerLayer = LayerMask.GetMask("PlayerLayer");
    }

    private void Start()
    {
        InvokeRepeating("DetectPlayer", 0.0f, 0.65f);
    }

    private void FixedUpdate()
    {
        if (isPlayerInRange)
        {
            var forceIntensity = Vector3.zero;
            var targetDirection =
                (Player.position - transform.position).normalized;

            switch (MovementType)
            {
                case MovementBehaviour.Constant:
                    forceIntensity = VelocityImpulse * targetDirection;
                    break;
                case MovementBehaviour.DecreaseOnDistance:
                    forceIntensity = VelocityImpulse * targetDirection /
                                     distanceToPlayer;
                    break;
                case MovementBehaviour.IncreaseOnDistance:
                    forceIntensity = VelocityImpulse * targetDirection *
                                     distanceToPlayer;
                    break;
                case MovementBehaviour.Fixed:
                    break;
            }

            // apply force on object rigidbody
            if (MovementType != MovementBehaviour.Fixed)
            {
                eRigidbody.AddForce(forceIntensity);
            }
            else
            {
                eRigidbody.velocity = targetDirection * VelocityImpulse *
                                       Time.fixedDeltaTime;
            }
        }
        else // slowly slow down
        {
            eRigidbody.velocity = Vector3.Lerp(eRigidbody.velocity,
                Vector3.zero, Time.fixedDeltaTime * OutRangeSlowdown);
        }
    }

    private void DetectPlayer()
    {
        var targetNormal = (transform.position - Planet.position).normalized;
        var capsuleB = Quaternion.FromToRotation(Vector3.up, targetNormal) *
                       CapsuleBegin;
        var capsuleE = Quaternion.FromToRotation(Vector3.up, targetNormal) *
                       CapsuleEnd;

        isPlayerInRange = Physics.CheckCapsule(transform.position + capsuleB,
            transform.position + capsuleE,
            CapsuleRadius, playerLayer);
        distanceToPlayer = isPlayerInRange
            ? UseSquareDistance
                ? Vector3.SqrMagnitude(transform.position - Player.position)
                : Vector3.Distance(transform.position, Player.position)
            : Mathf.Infinity;
    }

    private void OnDrawGizmosSelected()
    {
        // gravity opposite direction
        if (eRigidbody != null)
        {
            Gizmos.color = Color.cyan;
            DrawArrow.ForGizmo(transform.position, eRigidbody.velocity);
        }

        var targetNormal = (transform.position - Planet.position).normalized;
        var capsuleB = Quaternion.FromToRotation(Vector3.up, targetNormal) *
                       CapsuleBegin;
        var capsuleE = Quaternion.FromToRotation(Vector3.up, targetNormal) *
                       CapsuleEnd;
        // bomb player detection range
        DebugExtension.DrawCapsule(transform.position + capsuleB,
            transform.position + capsuleE,
            isPlayerInRange ? Color.red : Color.gray, CapsuleRadius);

        if (!isPlayerInRange)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,
            transform.position +
            (Player.position - transform.position).normalized *
            distanceToPlayer);
    }
}