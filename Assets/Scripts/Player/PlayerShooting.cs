using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {
    public GameObject BulletObject;
    public float BulletLifeTime = 2f;
    public float BulletSpeed = 5f;
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetButtonUp("Fire1"))
        {
            GameObject newProjectile = Instantiate(BulletObject, transform.position, transform.rotation) as GameObject;
            if (newProjectile == null) return;

            Rigidbody pRigidbody = newProjectile.GetComponent<Rigidbody>();

            if (pRigidbody != null)
            {
                // tag the projectile as a coming from the player
                newProjectile.tag = "Player";
                // move towards forward vec3
                pRigidbody.velocity = BulletSpeed * transform.forward;
            }

            Destroy(newProjectile, BulletLifeTime);
        }
	}
}
