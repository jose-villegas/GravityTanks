using UnityEngine;

[RequireComponent(typeof (PlanetInfo))]
public class SphericalGravity : MonoBehaviour
{
    private int _playerLayer;
    public PlanetInfo PlanetInformation;
    public LayerMask PullMasks;
    public float PullRadius = 5f;

    private void Awake()
    {
        _playerLayer = LayerMask.GetMask("PlayerLayer");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PullRadius);
    }

    private void FixedUpdate()
    {
        var colliders = Physics.OverlapSphere(transform.position, PullRadius, PullMasks);

        foreach (var other in colliders)
        {
            var oRigidBody = other.GetComponent<Rigidbody>();

            if (oRigidBody != null)
            {
                var forceDirection = (transform.position - other.transform.position).normalized;

                // player special case
                if ((1 << other.gameObject.layer & _playerLayer) > 0)
                {
                    var playerStick = other.gameObject.GetComponent<StickToPlanet>();

                    if ((playerStick.Status & StickToPlanet.StickStatus.Stranded) > 0)
                    {
                        oRigidBody.AddForce(forceDirection*PlanetInformation.Gravity);
                    }
                    else
                    {
                        oRigidBody.AddForce(-playerStick.PlanetCurrentNormal*PlanetInformation.Gravity);
                    }
                }
                else
                {
                    oRigidBody.AddForce(forceDirection*PlanetInformation.Gravity);
                }
            }
        }
    }
}