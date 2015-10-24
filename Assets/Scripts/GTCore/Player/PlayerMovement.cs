using GTCore.Camera;
using GTCore.Utils;

using UnityEngine;

namespace GTCore.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(StickToPlanet))]
    public class PlayerMovement : MonoBehaviour
    {
        private bool isWalking;
        private Vector3 movement;
        private Rigidbody playerRigidbody;
        private StickToPlanet stickToPlanet;
        private Vector3 turnDirection = Vector3.up;
        public float Damping = 8f;
        public CameraMovement GameCamera;
        public float JumpStrength = 2f;

        /// <summary>
        ///     Torque based solution to kill angular velocity
        ///     this value decides how fast this happens
        /// </summary>
        public float KillAngularVelocity = 0.1f;

        public float MovementSpeed = 5f;

        private void Awake()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            stickToPlanet = GetComponent<StickToPlanet>();
        }

        private void FixedUpdate()
        {
            var horizontalAxis = Input.GetAxis("Horizontal");
            var verticalAxis = Input.GetAxis("Vertical");
            var jumpAxis = Input.GetAxis("Jump");

            // flags
            isWalking = Mathf.Abs(horizontalAxis) > Mathf.Epsilon ||
                        Mathf.Abs(verticalAxis) > Mathf.Epsilon;
            // player movement calls
            Turn(horizontalAxis, verticalAxis);
            Move(horizontalAxis, verticalAxis);
            Jump(jumpAxis);
        }

        private void Move(float horizontal, float vertical)
        {
            if (!isWalking)
            {
                return;
            }

            movement = GameCamera.NonInterpolatedTransform.right * horizontal +
                       GameCamera.NonInterpolatedTransform.up * vertical;
            movement = movement.normalized * MovementSpeed * Time.fixedDeltaTime;

            playerRigidbody.MovePosition(playerRigidbody.transform.position + movement);
        }

        private void Turn(float horizontal, float vertical)
        {
            var mainCamera = UnityEngine.Camera.main.transform;
            // smooth rotation with damping parameter
            if (isWalking && mainCamera.up != Vector3.zero)
            {
                var newTurnDirection =
                    Quaternion.LookRotation(mainCamera.up,
                        stickToPlanet.PlanetCurrentNormal) *
                    new Vector3(horizontal, 0f, vertical);
                turnDirection = newTurnDirection == Vector3.zero
                    ? turnDirection
                    : newTurnDirection;
                var rotAround = Quaternion.LookRotation(turnDirection,
                    stickToPlanet.PlanetCurrentNormal);
                // look forward to input plane direction
                playerRigidbody.MoveRotation(
                    Quaternion.Slerp(playerRigidbody.transform.rotation, rotAround,
                        Damping * Time.deltaTime));
            }
            else
            {
                var rotAround = Quaternion.FromToRotation(transform.up,
                    stickToPlanet.PlanetCurrentNormal);
                // rot to up vector based on gravity
                playerRigidbody.MoveRotation(
                    Quaternion.Slerp(playerRigidbody.transform.rotation,
                        rotAround * playerRigidbody.transform.rotation,
                        Damping * Time.deltaTime));
            }

            // kill angular rotation from accumulated force on rigidbody
            playerRigidbody.AddTorque(-playerRigidbody.angularVelocity * KillAngularVelocity);
        }

        private void Jump(float jump)
        {
            if (jump <= 0f)
            {
                return;
            }

            playerRigidbody.velocity = transform.up * jump * JumpStrength;
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                // velocity vector direction
                Gizmos.color = Color.magenta;
                DrawArrow.ForGizmo(transform.position, playerRigidbody.velocity);

                // axis input plane
                Gizmos.color = Color.red;
                DebugExtension.DrawCircle(transform.position,
                    stickToPlanet.PlanetCurrentNormal, Color.red, 1f);
            }

            // draw current input position
            Gizmos.DrawCube(transform.position + turnDirection, Vector3.one * 0.15f);
        }
    }
}