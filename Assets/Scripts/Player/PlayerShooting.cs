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
            var newProjectile = Instantiate(physicsBullet, transform.position, transform.rotation);
            Rigidbody pRigidbody = ((GameObject)newProjectile).GetComponent<Rigidbody>();

            if (pRigidbody != null)
            {
                Vector3 velocityVector = new Vector3(0f, 0f, bulletSpeed);
                pRigidbody.velocity = bulletSpeed * transform.forward;
            }
            Destroy(newProjectile, bulletLifeTime);
        }
	}
}
