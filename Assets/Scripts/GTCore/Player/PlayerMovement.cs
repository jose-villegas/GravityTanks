using GTCore.Camera;
using GTUtils;
using UnityEngine;

namespace GTCore.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(StickToPlanet))]
    public class PlayerMovement : MonoBehaviour
    {
        private bool _isWalking;
        private Rigidbody _playerRigidbody;
        private StickToPlanet _stickToPlanet;
        private Vector3 _turnDirection = Vector3.up;

        /// <summary>
        ///     Softens the player rotation / turning
        /// </summary>
        [Tooltip("Softens the player turning movement")]
        public float Damping = 8f;

        public CameraMovement GameCamera;
        public float JumpStrength = 2f;

        /// <summary>
        ///     Torque based solution to kill angular velocity
        ///     this value decides how fast this happens
        /// </summary>
        [Tooltip("Indicates how fast the angular velocity becomes zero, x < 1")]
        public float KillAngularVelocity = 0.1f;

        public float MovementSpeed = 5f;

        private void Awake()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _stickToPlanet = GetComponent<StickToPlanet>();
        }

        private void FixedUpdate()
        {
            var horizontalAxis = Input.GetAxis("Horizontal");
            var verticalAxis = Input.GetAxis("Vertical");
            var jumpAxis = Input.GetAxis("Jump");

            // flags
            _isWalking = Mathf.Abs(horizontalAxis) > Mathf.Epsilon ||
                         Mathf.Abs(verticalAxis) > Mathf.Epsilon;
            // player movement calls
            Turn(horizontalAxis, verticalAxis);
            Move(horizontalAxis, verticalAxis);
            Jump(jumpAxis);
        }

        private void Move(float horizontal, float vertical)
        {
            if ( !_isWalking )
            {
                return;
            }

            var _movement = GameCamera.NonInterpolatedTransform.right *
                            horizontal +
                            GameCamera.NonInterpolatedTransform.up * vertical;
            _movement = _movement.normalized * MovementSpeed *
                        Time.fixedDeltaTime;

            _playerRigidbody.MovePosition(_playerRigidbody.transform.position +
                                          _movement);
        }

        private void Turn(float horizontal, float vertical)
        {
            var mainCamera = GameCamera.NonInterpolatedTransform;
            // smooth rotation with damping parameter
            if ( _isWalking && mainCamera.up != Vector3.zero )
            {
                var newTurnDirection =
                    Quaternion.LookRotation(mainCamera.up,
                        _stickToPlanet.PlanetCurrentNormal) *
                    new Vector3(horizontal, 0f, vertical);
                _turnDirection = newTurnDirection == Vector3.zero
                    ? _turnDirection
                    : newTurnDirection;
                var rotAround = Quaternion.LookRotation(_turnDirection,
                    _stickToPlanet.PlanetCurrentNormal);
                // look forward to input plane direction
                _playerRigidbody.MoveRotation(
                    Quaternion.Slerp(_playerRigidbody.transform.rotation,
                        rotAround,
                        Damping * Time.deltaTime));
            }
            else
            {
                var rotAround = Quaternion.FromToRotation(transform.up,
                    _stickToPlanet.PlanetCurrentNormal);
                // rotate to up vector based on gravity
                _playerRigidbody.MoveRotation(
                    Quaternion.Slerp(_playerRigidbody.transform.rotation,
                        rotAround * _playerRigidbody.transform.rotation,
                        Damping * Time.deltaTime));
            }

            // kill angular rotation from accumulated force on rigidbody
            _playerRigidbody.AddTorque(-_playerRigidbody.angularVelocity *
                                       KillAngularVelocity);
        }

        private void Jump(float jump)
        {
            if ( jump <= 0f )
            {
                return;
            }

            _playerRigidbody.velocity = transform.up * jump * JumpStrength;
        }

        private void OnDrawGizmosSelected()
        {
            if ( Application.isPlaying )
            {
                // velocity vector direction
                Gizmos.color = Color.magenta;
                DrawArrow.ForGizmo(transform.position, _playerRigidbody.velocity);

                // axis input plane
                Gizmos.color = Color.red;
                DebugExtension.DrawCircle(transform.position,
                    _stickToPlanet.PlanetCurrentNormal, Color.red, 1f);
            }

            // draw current input position
            Gizmos.DrawCube(transform.position + _turnDirection,
                Vector3.one * 0.15f);
        }
    }
}