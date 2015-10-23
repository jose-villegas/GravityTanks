using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 DirectionToTarget = Vector3.zero;
    public Vector3 PositionToTarget = Vector3.zero;
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
        var targetDirection = (Target.position - transform.position).normalized;

        var targetCamPos = Target.position + Target.right*PositionToTarget.x + Target.up*PositionToTarget.y +
                           Target.forward*PositionToTarget.z;
        transform.position = Vector3.Slerp(transform.position, targetCamPos, Smoothing*Time.deltaTime);

        var lookToPlayer = Quaternion.LookRotation(targetDirection + DirectionToTarget, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookToPlayer, Smoothing*Time.deltaTime);

        NonInterpolatedTransform.position = targetCamPos;
        NonInterpolatedTransform.rotation = lookToPlayer;
    }

    private void OnDrawGizmosSelected()
    {
        // line to view target
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, Target.position);
        // line from position to final position after smoothing
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, NonInterpolatedTransform.position);
            Gizmos.DrawSphere(NonInterpolatedTransform.position, 0.25f);
        }
    }
}