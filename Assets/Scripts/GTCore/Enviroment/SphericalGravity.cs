using GTCore.Player;

using UnityEngine;

namespace GTCore.Enviroment
{
    [RequireComponent(typeof(PlanetInfo))]
    public class SphericalGravity : MonoBehaviour
    {
        private int playerLayer;

        [Tooltip("Moves the sphere center")]
        public Vector3 MovePosition = Vector3.zero;

        public PlanetInfo PlanetInformation;
        public LayerMask PullMasks;
        public float PullRadius = 5f;

        [Tooltip("Calculates the center of mass with all children's rigidbodies")]
        public bool
            UseCenterOfMass = false;

        private void Awake()
        {
            playerLayer = LayerMask.GetMask("PlayerLayer");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(GetCenter(), PullRadius);
            // draw center of mass
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(GetCenter(), 0.25f);
        }

        public Vector3 GetCenter()
        {
            var position = UseCenterOfMass
                ? CenterOfMass(transform.GetComponentsInChildren<Rigidbody>())
                : transform.position;
            position += MovePosition;
            return position;
        }

        private void FixedUpdate()
        {
            var colliders = Physics.OverlapSphere(GetCenter(), PullRadius, PullMasks);

            foreach (var other in colliders)
            {
                var oRigidBody = other.GetComponent<Rigidbody>();

                if (oRigidBody == null)
                {
                    continue;
                }

                var forceDirection =
                    (transform.position - other.transform.position).normalized;

                // player special case
                if ((1 << other.gameObject.layer & playerLayer) > 0)
                {
                    var playerStick = other.gameObject.GetComponent<StickToPlanet>();

                    if ((playerStick.Status & StickToPlanet.StickStatus.Stranded) >
                        0)
                    {
                        oRigidBody
                            .AddForce(forceDirection *
                                      PlanetInformation.Gravity);
                    }
                    else
                    {
                        oRigidBody.AddForce(-playerStick.PlanetCurrentNormal *
                                            PlanetInformation.Gravity);
                    }
                }
                else
                {
                    oRigidBody.AddForce(forceDirection * PlanetInformation.Gravity);
                }
            }
        }

        public static Vector3 CenterOfMass(Rigidbody[] rbArray)
        {
            var com = Vector3.zero;
            var c = 0f;

            foreach (var goRigidbody in rbArray)
            {
                com += goRigidbody.worldCenterOfMass * goRigidbody.mass;
                c += goRigidbody.mass;
            }

            return com / c;
        }
    }
}