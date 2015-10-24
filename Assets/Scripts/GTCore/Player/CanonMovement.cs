using UnityEngine;

namespace GTCore.Player
{
    public class CanonMovement : MonoBehaviour
    {
        private int _inputMask;

        /// <summary>
        ///     The canon will rotate around the canong owner
        /// </summary>
        public Transform CanonOwner;

        /// <summary>
        ///     Indicates the seperation between the canon owner and the canon
        /// </summary>
        [Tooltip("Seperation between the canon owner and the canon object")]
        public float CanonSeparation = 0.5f;

        /// <summary>
        ///     Damps the canon rotation to make it rotate around softly
        /// </summary>
        [Tooltip("Damps the canon rotation to make it rotate around softly")]
        public float TurningDamping = 8.0f;

        private void Start()
        {
            _inputMask = LayerMask.GetMask("InputCapture");
        }

        // Update is called once per frame
        private void Update()
        {
            Move();
        }

        private void Move()
        {
            // set cannon to proper distance to the player or target
            transform.position = CanonOwner.position +
                                 transform.forward * CanonSeparation;

            var camRay =
                UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit inputPlaneHit;

            if (
                !Physics.Raycast(camRay, out inputPlaneHit, Mathf.Infinity,
                    _inputMask) )
            {
                return;
            }

            var mouseDirection =
                (inputPlaneHit.point - transform.position).normalized;
            var toMouse = Quaternion.LookRotation(mouseDirection,
                CanonOwner.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toMouse,
                TurningDamping * Time.deltaTime);
        }
    }
}