using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {
    public GameObject physicsBullet;
    public float bulletLifeTime = 2f;
    public float bulletSpeed = 5f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetButtonUp("Fire1"))
        {
            GameObject newProjectile = Instantiate(physicsBullet, transform.position, transform.rotation) as GameObject;
            // tag the projectile as a coming from the player
            newProjectile.tag = "Player";

            Rigidbody pRigidbody = newProjectile.GetComponent<Rigidbody>();

            if (pRigidbody != null)
            {
                pRigidbody.velocity = bulletSpeed * transform.forward;
            }

            Destroy(newProjectile, bulletLifeTime);
        }
	}
}
