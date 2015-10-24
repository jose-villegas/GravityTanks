using UnityEngine;

namespace GTCore.Camera
{
    /// <summary>
    ///     Controls the main camera movement behaviour
    /// </summary>
    public class CameraMovement : MonoBehaviour
    {
        /// <summary>
        ///     Moves the position of the camera relative to the target position
        /// </summary>
        [Tooltip("Moves the position of the camera relative to the target")]
        public Vector3 MovePosition = Vector3.zero;

        /// <summary>
        ///     Moves the view target position, changing the resulting direction
        ///     vector
        /// </summary>
        [Tooltip("Moves the view target direction")]
        public Vector3 MoveTarget = Vector3.zero;

        /// <summary>
        ///     Damps the camera movement making the movement softly reach its
        ///     final
        ///     position
        /// </summary>
        [Tooltip("Softens the camera movement")]
        public float Smoothing = 5f;

        /// <summary>
        ///     The target which the camera will be looking at
        /// </summary>
        public Transform Target;

        /// <summary>
        ///     Represents the camera final position without movement smoothing
        /// </summary>
        public Transform NonInterpolatedTransform { get; private set; }

        private void Start()
        {
            var go = new GameObject("NonInterpolatedTransform");
            NonInterpolatedTransform = go.transform;
            go.transform.SetParent(transform, false);
        }

        private void LateUpdate()
        {
            // move target position with MovePosition parameters
            var targetCamPos = Target.position +
                               Target.right * MovePosition.x +
                               Target.up * MovePosition.y +
                               Target.forward * MovePosition.z;
            transform.position = Vector3.Slerp(transform.position, targetCamPos,
                Smoothing * Time.deltaTime);

            // modify target direction with move MoveTarget parameters
            var targetDirection = (Target.position +
                                   Target.right * MoveTarget.x +
                                   Target.up * MoveTarget.y +
                                   Target.forward * MoveTarget.z -
                                   transform.position).normalized;
            var lookToPlayer = Quaternion.LookRotation(targetDirection,
                transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                lookToPlayer, Smoothing * Time.deltaTime);

            NonInterpolatedTransform.position = targetCamPos;
            NonInterpolatedTransform.rotation = lookToPlayer;
        }

        private void OnDrawGizmosSelected()
        {
            // line to view target
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, Target.position +
                                                Target.right * MoveTarget.x +
                                                Target.up * MoveTarget.y +
                                                Target.forward * MoveTarget.z);
            // line from position to final position after smoothing
            if ( Application.isPlaying )
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position,
                    NonInterpolatedTransform.position);
                Gizmos.DrawSphere(NonInterpolatedTransform.position, 0.25f);
            }
        }
    }
}