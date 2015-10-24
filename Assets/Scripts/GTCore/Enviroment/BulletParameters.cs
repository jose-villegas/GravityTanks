using GTUtils;

using UnityEngine;

namespace GTCore.Enviroment
{
    public class BulletParameters : MonoBehaviour
    {
        /// <summary>
        ///     Bullets have an initial owner to avoid self collision
        /// </summary>
        public float TimeToIgnoreOwner = 1f;

        private void Start()
        {
            Invoke("ForgetOwner", TimeToIgnoreOwner);
        }

        private void ForgetOwner()
        {
            gameObject.tag = "Untagged";
        }

        private void OnTriggerEnter(Collider other)
        {
            if ( other.gameObject.tag == gameObject.tag )
            {
                return;
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            DrawArrow.ForGizmo(transform.position,
                GetComponent<Rigidbody>().velocity);
        }
    }
}