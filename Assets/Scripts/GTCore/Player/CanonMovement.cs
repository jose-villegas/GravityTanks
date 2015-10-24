using UnityEngine;

namespace GTCore.Player
{
    public class CanonMovement : MonoBehaviour
    {
        public float CamRayLength = 100f;
        public float CanonDistance = 0.5f;
        public Transform RotateAround;
        public float TurningSpeed = 8.0f;
        public int InputMask { get; private set; }

        private void Start()
        {
            InputMask = LayerMask.GetMask("InputCapture");
        }

        // Update is called once per frame
        private void Update()
        {
            Move();
        }

        private void Move()
        {
            // set cannon to proper distance to the player or target
            transform.position = RotateAround.position +
                                 transform.forward * CanonDistance;

            var camRay =
                UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit inputPlaneHit;

            if (
                !Physics.Raycast(camRay, out inputPlaneHit, CamRayLength,
                    InputMask) )
            {
                return;
            }

            var mouseDirection =
                (inputPlaneHit.point - transform.position).normalized;
            var toMouse = Quaternion.LookRotation(mouseDirection,
                RotateAround.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toMouse,
                TurningSpeed * Time.deltaTime);
        }
    }
}