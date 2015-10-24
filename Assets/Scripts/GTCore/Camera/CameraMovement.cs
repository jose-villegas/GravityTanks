using UnityEngine;

namespace GTCore.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public Vector3 MovePosition = Vector3.zero;
        public Vector3 MoveTarget = Vector3.zero;
        public float Smoothing = 5f;
        public Transform Target;

        public Transform NonInterpolatedTransform { get; private set; }

        private void Start()
        {
            var go = new GameObject("NonInterpolatedTransform");
            NonInterpolatedTransform = go.transform;
            go.transform.SetParent(transform, false);
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            var targetCamPos = Target.position +
                               Target.right * MovePosition.x +
                               Target.up * MovePosition.y +
                               Target.forward * MovePosition.z;
            transform.position = Vector3.Slerp(transform.position, targetCamPos,
                Smoothing * Time.deltaTime);

            var targetDirection =
                ( Target.position + MoveTarget - transform.position ).normalized;
            var lookToPlayer = Quaternion.LookRotation(targetDirection,
                transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                lookToPlayer,
                Smoothing * Time.deltaTime);

            NonInterpolatedTransform.position = targetCamPos;
            NonInterpolatedTransform.rotation = lookToPlayer;
        }

        private void OnDrawGizmosSelected()
        {
            // line to view target
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, Target.position + MoveTarget);
            // line from position to final position after smoothing
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position,
                    NonInterpolatedTransform.position);
                Gizmos.DrawSphere(NonInterpolatedTransform.position, 0.25f);
            }
        }
    }
}